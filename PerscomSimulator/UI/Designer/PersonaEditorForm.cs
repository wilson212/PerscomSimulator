using Perscom.Database;
using Perscom.Simulation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.Charting;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Perscom
{
    public partial class PersonaEditorForm : RadForm
    {
        /// <summary>
        /// Contains a list of all Attribute RatingTrackBars that affect soldier min and max spawn ratings
        /// </summary>
        private Dictionary<AttributeType, RadTrackBar> RatingTrackBars { get; set; }

        /// <summary>
        /// The Persona entity being edited. Null if creating a new one.
        /// </summary>
        private Persona ExistingPersona { get; set; }

        /// <summary>
        /// Indicates whether this is a new Persona (true) or editing an existing one (false).
        /// </summary>
        private bool IsNewPersona { get; set; }

        /// <summary>
        /// Constructor for creating a brand new Persona.
        /// </summary>
        public PersonaEditorForm() : this(null)
        {
            // Create components and apply theme
            InitializeComponent();
            FormStyling.ApplyControlsTheme(Controls);
        }

        /// <summary>
        /// Constructor for editing an existing Persona, or creating a new one if null is passed.
        /// </summary>
        public PersonaEditorForm(Persona existing)
        {
            // Apply form styling and create controls
            InitializeComponent();
            FormStyling.ApplyControlsTheme(this.Controls);
            //FormStyling.StyleButtonFluentBlue(saveButton);

            ExistingPersona = existing;
            IsNewPersona = (existing == null);

            // Store trackbars
            RatingTrackBars = new Dictionary<AttributeType, RadTrackBar>()
            {
                { AttributeType.Leadership, LeadershipTrackBar },
                { AttributeType.Composure, StabilityTrackBar },
                { AttributeType.Marksmanship, MarksmanTrackBar },
                { AttributeType.Fitness, FitnessTrackBar },
                { AttributeType.Teamwork, TeamWorkTrackBar },
                { AttributeType.Discipline, CommTrackBar },
                { AttributeType.Ambition, AmbitionTrackBar },
                { AttributeType.Extraversion, ExtraversionTrackBar },
                { AttributeType.Conscientiousness, ConTrackBar },
                { AttributeType.Agreeableness, AgreeableTrackBar },
                { AttributeType.Mindfullness, MindfullTrackBar },
                { AttributeType.Courage, OpennessTrackBar },
            };

            // If editing an existing Persona, load its data into the form
            if (!IsNewPersona)
                LoadPersonaIntoForm();

            // Fill star ratings
            FillStarRatings();

            // Plot initial charts
            PlotAgeBellCurve();
            PlotCareerBellCurve();

            // Bind attribute change events
            foreach (var track in RatingTrackBars)
            {
                track.Value.TrackBarElement.Ranges[0].PropertyChanged += StatTrack_PropertyChanged;
            }

            IntellectTrackBar.TrackBarElement.Ranges[0].PropertyChanged += StatTrack_PropertyChanged;

            // Bind age chart refresh events
            minAgeSpinEditor.ValueChanged += AgeParameter_ValueChanged;
            avgAgeSpinEditor.ValueChanged += AgeParameter_ValueChanged;
            maxAgeSpinEditor.ValueChanged += AgeParameter_ValueChanged;
            ageSkewLeftTrackBar.ValueChanged += AgeParameter_ValueChanged;
            ageSkewRightTrackBar.ValueChanged += AgeParameter_ValueChanged;

            // Bind career chart refresh events
            minCareerSpinEditor.ValueChanged += CareerParameter_ValueChanged;
            avgCareerSpinEditor.ValueChanged += CareerParameter_ValueChanged;
            maxCareerSpinEditor.ValueChanged += CareerParameter_ValueChanged;
            careerSkewLeftTrackBar.ValueChanged += CareerParameter_ValueChanged;
            careerSkewRightTrackBar.ValueChanged += CareerParameter_ValueChanged;

            // Enable tooltips on charts
            var ageTooltip = new ChartTooltipController();
            ageChartView.Controllers.Add(ageTooltip);

            var careerTooltip = new ChartTooltipController();
            careerChartView.Controllers.Add(careerTooltip);
        }

        /// <summary>
        /// Loads an existing Persona's data from the database into all form controls.
        /// </summary>
        private void LoadPersonaIntoForm()
        {
            var persona = ExistingPersona;

            // General tab
            personaNameTextBox.Text = persona.Name;
            ProbabilityTrackBar.Value = persona.Probability;
            GenderTrackBar.Value = persona.FemaleGenderRatio;

            // Age controls
            minAgeSpinEditor.Value = persona.MinAge;
            avgAgeSpinEditor.Value = persona.AverageAge;
            maxAgeSpinEditor.Value = persona.MaxAge;
            ageSkewLeftTrackBar.Value = (float)(persona.SkewAgeLeft * 10.0);
            ageSkewRightTrackBar.Value = (float)(persona.SkewAgeRight * 10.0);

            // Career length controls (loaded from the referenced CareerLength entity)
            using var db = new AppDatabase();
            var careerLength = db.CareerLengths.Find(persona.CareerLengthId);
            if (careerLength != null)
            {
                minCareerSpinEditor.Value = careerLength.MinimumMonths;
                avgCareerSpinEditor.Value = careerLength.AverageMonths;
                maxCareerSpinEditor.Value = careerLength.MaximumMonths;
                careerSkewLeftTrackBar.Value = (float)(careerLength.SkewLeft * 10.0);
                careerSkewRightTrackBar.Value = (float)(careerLength.SkewRight * 10.0);
            }

            // Load attribute trackbar ranges from PersonaAttributes
            var attributes = db.PersonaAttributes.FindAll(persona.Id);
            foreach (var attr in attributes)
            {
                if (RatingTrackBars.TryGetValue(attr.AttributeId, out var trackBar))
                {
                    trackBar.Ranges[0].Start = attr.MinSpawnValue;
                    trackBar.Ranges[0].End = attr.MaxSpawnValue;
                }
                else if (attr.AttributeId == AttributeType.Intellect)
                {
                    IntellectTrackBar.Ranges[0].Start = attr.MinSpawnValue;
                    IntellectTrackBar.Ranges[0].End = attr.MaxSpawnValue;
                }
                else if (attr.AttributeId == AttributeType.Improvability)
                {
                    ImproveTrackBar.Ranges[0].Start = attr.MinSpawnValue;
                    ImproveTrackBar.Ranges[0].End = attr.MaxSpawnValue;
                }
                else if (attr.AttributeId == AttributeType.Adaptability)
                {
                    AdaptTrackBar.Ranges[0].Start = attr.MinSpawnValue;
                    AdaptTrackBar.Ranges[0].End = attr.MaxSpawnValue;
                }
            }

            // Update labels
            GenderTrackBar_ValueChanged(null, EventArgs.Empty);
            ProbabilityTrackBar_ValueChanged(null, EventArgs.Empty);
        }

        #region Chart Plotting

        /// <summary>
        /// Plots the asymmetric bell curve for the Age distribution on the ageChartView.
        /// </summary>
        private void PlotAgeBellCurve()
        {
            int min = (int)minAgeSpinEditor.Value;
            int avg = (int)avgAgeSpinEditor.Value;
            int max = (int)maxAgeSpinEditor.Value;
            double skewLeft = (double)ageSkewLeftTrackBar.Value / 10.0;
            double skewRight = (double)ageSkewRightTrackBar.Value / 10.0;

            PlotBellCurve(ageChartView, min, avg, max, skewLeft, skewRight);
        }

        /// <summary>
        /// Plots the asymmetric bell curve for the Career Length distribution on the careerChartView.
        /// </summary>
        private void PlotCareerBellCurve()
        {
            int min = (int)minCareerSpinEditor.Value;
            int avg = (int)avgCareerSpinEditor.Value;
            int max = (int)maxCareerSpinEditor.Value;
            double skewLeft = (double)careerSkewLeftTrackBar.Value / 10.0;
            double skewRight = (double)careerSkewRightTrackBar.Value / 10.0;

            PlotBellCurve(careerChartView, min, avg, max, skewLeft, skewRight);
        }

        /// <summary>
        /// Generic method to plot an asymmetric bell curve on a RadChartView using MathUtils.
        /// </summary>
        private void PlotBellCurve(RadChartView chartView, int min, int avg, int max, double skewLeft, double skewRight)
        {
            if (chartView.Series.Count == 0) return;

            var lineSeries = chartView.Series[0] as LineSeries;
            if (lineSeries == null) return;

            // Create the dot series on first call if it doesn't exist yet
            if (chartView.Series.Count < 2)
            {
                var dotSeries = new LineSeries();
                dotSeries.ShowLabels = false;
                dotSeries.BorderWidth = 0;
                dotSeries.PointSize = new SizeF(10, 10);
                dotSeries.CombineMode = ChartSeriesCombineMode.None;
                dotSeries.HorizontalAxis = lineSeries.HorizontalAxis;
                dotSeries.VerticalAxis = lineSeries.VerticalAxis;
                chartView.Series.Add(dotSeries);
            }

            var smoothSeries = lineSeries;
            var markerSeries = chartView.Series[1] as LineSeries;

            smoothSeries.DataPoints.Clear();
            markerSeries?.DataPoints.Clear();

            if (min >= max || avg < min || avg > max) return;

            int range = max - min;
            double divisor = range <= 10 ? 3.0 : 6.0;
            double baseStdDev = Math.Max(1.0, range / divisor);

            double leftStdDev = baseStdDev * skewLeft;
            double rightStdDev = baseStdDev * skewRight;

            if (leftStdDev < 0.01) leftStdDev = 0.01;
            if (rightStdDev < 0.01) rightStdDev = 0.01;

            // Dynamically choose step sizes to cap the number of points
            // Smooth line: aim for ~100-200 points max
            int smoothStep = Math.Max(1, range / 150);

            // Dot markers: aim for ~15-25 dots max
            int dotStep;
            if (range <= 50)
                dotStep = 5;
            else if (range <= 200)
                dotStep = 10;
            else if (range <= 500)
                dotStep = 25;
            else if (range <= 1500)
                dotStep = 50;
            else
                dotStep = 100;

            // Smooth line
            for (int x = min; x <= max; x += smoothStep)
            {
                double y = MathUtils.GetAsymmetricBellCurveY(x, avg, leftStdDev, rightStdDev);
                smoothSeries.DataPoints.Add(new CategoricalDataPoint
                {
                    Value = y * 100,
                    Category = x
                });
            }

            // Ensure the last point is always plotted for a clean end
            if ((max - min) % smoothStep != 0)
            {
                double yEnd = MathUtils.GetAsymmetricBellCurveY(max, avg, leftStdDev, rightStdDev);
                smoothSeries.DataPoints.Add(new CategoricalDataPoint
                {
                    Value = yEnd * 100,
                    Category = max
                });
            }

            // Dot markers every dotStep
            if (markerSeries != null)
            {
                int start = ((min + dotStep - 1) / dotStep) * dotStep; // Round up to nearest multiple
                for (int x = start; x <= max; x += dotStep)
                {
                    double y = MathUtils.GetAsymmetricBellCurveY(x, avg, leftStdDev, rightStdDev);
                    double yPercent = y * 100;

                    markerSeries.DataPoints.Add(new CategoricalDataPoint
                    {
                        Value = yPercent,
                        Category = x,
                        Label = $"{x}: {yPercent:0.##}%"
                    });
                }
            }

            if (smoothSeries.VerticalAxis is LinearAxis linearAxis)
            {
                linearAxis.LabelFormat = "{0:0.#}%";
            }

            // Thin out X-axis labels to prevent overcrowding
            if (smoothSeries.HorizontalAxis is CategoricalAxis catAxis)
            {
                range = max - min;

                if (range <= 40)
                {
                    catAxis.LabelInterval = 1;
                }
                else if (range <= 80)
                {
                    catAxis.LabelInterval = 2;
                }
                else if (range <= 150)
                {
                    catAxis.LabelInterval = 5;
                }
                else if (range <= 200)
                {
                    catAxis.LabelInterval = 10;
                }
                else if (range <= 500)
                {
                    catAxis.LabelInterval = 25;
                }
                else
                {
                    catAxis.LabelInterval = 50;
                }
            }
        }

        private void AgeParameter_ValueChanged(object sender, EventArgs e)
        {
            PlotAgeBellCurve();
        }

        private void CareerParameter_ValueChanged(object sender, EventArgs e)
        {
            PlotCareerBellCurve();
        }

        #endregion Chart Plotting

        private void StatTrack_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FillStarRatings();
        }

        /// <summary>
        /// Whenever an Attribute Trackbar value is changed, this method changes the min, max and full
        /// star potential ratings on the form
        /// </summary>
        private void FillStarRatings()
        {
            double best = RatingTrackBars.Count * 20;
            int totalMin = 0, totalMax = 0;

            foreach (var track in RatingTrackBars.Values)
            {
                totalMin += (int)track.Ranges[0].Start;
                totalMax += (int)track.Ranges[0].End;
            }

            MinSpawnRating.Value = Math.Round(100 * (totalMin / best), 2);
            MaxSpawnRating.Value = Math.Round(100 * (totalMax / best), 2);
            FullSpawnRating.Value = Math.Round(100 * (IntellectTrackBar.Ranges[0].End / 20), 2);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // === Validation ===
            string name = personaNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                RadMessageBox.Show("Please enter a name for this Persona.",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            if ((int)ProbabilityTrackBar.Value <= 0)
            {
                RadMessageBox.Show("Spawn probability must be greater than zero.",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            int minAge = (int)minAgeSpinEditor.Value;
            int avgAge = (int)avgAgeSpinEditor.Value;
            int maxAge = (int)maxAgeSpinEditor.Value;

            if (avgAge < minAge || avgAge > maxAge)
            {
                RadMessageBox.Show("Average age must be between minimum and maximum age.",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            int minCareer = (int)minCareerSpinEditor.Value;
            int avgCareer = (int)avgCareerSpinEditor.Value;
            int maxCareer = (int)maxCareerSpinEditor.Value;

            if (avgCareer < minCareer || avgCareer > maxCareer)
            {
                RadMessageBox.Show("Average career length must be between minimum and maximum.",
                    "Validation Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            // === Save ===
            using var db = new AppDatabase();
            using var transaction = db.BeginTransaction();

            try
            {
                // --- CareerLength entity ---
                CareerLength careerLength;

                if (IsNewPersona)
                {
                    careerLength = db.CareerLengths.Create();
                }
                else
                {
                    careerLength = db.CareerLengths.Find(ExistingPersona.CareerLengthId);
                }

                careerLength.Name = name + " Career";
                careerLength.MinimumMonths = minCareer;
                careerLength.AverageMonths = avgCareer;
                careerLength.MaximumMonths = maxCareer;
                careerLength.SkewLeft = (double)careerSkewLeftTrackBar.Value / 10.0;
                careerLength.SkewRight = (double)careerSkewRightTrackBar.Value / 10.0;

                if (IsNewPersona)
                    db.CareerLengths.Add(careerLength);
                else
                    db.CareerLengths.Update(careerLength);

                // --- Persona entity ---
                Persona persona;

                if (IsNewPersona)
                {
                    persona = db.Personas.Create();
                }
                else
                {
                    persona = ExistingPersona;
                }

                persona.Name = name;
                persona.Probability = (int)ProbabilityTrackBar.Value;
                persona.FemaleGenderRatio = (int)GenderTrackBar.Value;
                persona.CareerLengthId = careerLength.Id;
                persona.MinAge = minAge;
                persona.MaxAge = maxAge;
                persona.AverageAge = avgAge;
                persona.SkewAgeLeft = (double)ageSkewLeftTrackBar.Value / 10.0;
                persona.SkewAgeRight = (double)ageSkewRightTrackBar.Value / 10.0;
                persona.MinTraits = 0;  // TODO: Add trait controls if needed
                persona.MaxTraits = 0;
                persona.AverageTraits = 0;
                persona.SkewTraitsLeft = 1.0;
                persona.SkewTraitsRight = 1.0;

                if (IsNewPersona)
                {
                    db.Personas.Add(persona);
                    IsNewPersona = false;
                }
                else
                {
                    db.Personas.Update(persona);
                }

                // --- PersonaAttributes ---
                // Delete old attributes, then re-insert
                var oldAttributes = db.PersonaAttributes.FindAll(persona.Id).ToArray();
                if (oldAttributes.Length > 0)
                    db.PersonaAttributes.RemoveRange(oldAttributes);

                // Save all trackbar-based attributes (Skills + Personality)
                foreach (var kvp in RatingTrackBars)
                {
                    var attr = db.PersonaAttributes.Create();
                    attr.AttributeId = kvp.Key;
                    SavePersonaAttribute(db, persona.Id, kvp.Key, (int)kvp.Value.Ranges[0].Start, (int)kvp.Value.Ranges[0].End);
                }

                // Save Intellect
                SavePersonaAttribute(db, persona.Id, AttributeType.Intellect,
                    (int)IntellectTrackBar.Ranges[0].Start, (int)IntellectTrackBar.Ranges[0].End);

                // Save Improvability
                SavePersonaAttribute(db, persona.Id, AttributeType.Improvability,
                    (int)ImproveTrackBar.Ranges[0].Start, (int)ImproveTrackBar.Ranges[0].End);

                // Save Adaptability
                SavePersonaAttribute(db, persona.Id, AttributeType.Adaptability,
                    (int)AdaptTrackBar.Ranges[0].Start, (int)AdaptTrackBar.Ranges[0].End);

                transaction.Commit();

                ExistingPersona = persona;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                RadMessageBox.Show($"Failed to save Persona: {ex.Message}",
                    "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        /// <summary>
        /// Helper to create and insert a single PersonaAttribute record.
        /// </summary>
        private void SavePersonaAttribute(AppDatabase db, int personaId, AttributeType attrType, int minVal, int maxVal)
        {
            var entity = db.PersonaAttributes.Create();
            // Since PersonaId is a protected set PrimaryKey, we use the CrossLite Create() pattern
            // which should allow setting via the entity before Add
            entity.PersonaId = personaId;
            entity.AttributeId = attrType;
            entity.MinSpawnValue = minVal;
            entity.MaxSpawnValue = maxVal;
            db.PersonaAttributes.Add(entity);
        }

        #region Paint Events

        private void HeaderPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormHeader(headerPanel, e);
            base.OnPaint(e);
        }

        private void BottomPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormFooterDark(bottomPanel, e);
            base.OnPaint(e);
        }

        #endregion Paint Events

        #region Labels

        private void GenderTrackBar_LabelFormatting(object sender, LabelFormattingEventArgs e)
        {
            if (e.LabelElement.IsTopLeft)
            {
                if (int.TryParse(e.LabelElement.Text, out int number))
                {
                    switch (number)
                    {
                        case 0:
                            e.LabelElement.Text = "Male";
                            break;
                        case 25:
                            e.LabelElement.Text = "75/25";
                            break;
                        case 50:
                            e.LabelElement.Text = "50/50";
                            break;
                        case 75:
                            e.LabelElement.Text = "25/75";
                            break;
                        case 100:
                            e.LabelElement.Text = "Female";
                            break;
                    }
                }
            }
        }
        private void Rating_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            RadRatingElement element = sender as RadRatingElement;
            if (element == null) return;

            var textValue = Math.Round((double)element.Value / 20, 2);
            e.ToolTipText = textValue.ToString();
        }

        private void GenderTrackBar_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            int val = 100 - (int)GenderTrackBar.Value;
            string p1 = val + "% Male, ";
            string p2 = ((int)GenderTrackBar.Value) + "% Female";
            e.ToolTipText = p1 + p2;
        }

        private void GenderTrackBar_ValueChanged(object sender, EventArgs e)
        {
            int val = 100 - (int)GenderTrackBar.Value;
            string p1 = val + "% Male, ";
            string p2 = ((int)GenderTrackBar.Value) + "% Female";
            GenderRatioLabel.Text = p1 + p2;
        }

        private void ProbabilityTrackBar_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            int val = (int)ProbabilityTrackBar.Value;
            e.ToolTipText = val.ToString();
        }

        private void ProbabilityTrackBar_ValueChanged(object sender, EventArgs e)
        {
            SpawnProbLabel.Text = ((int)ProbabilityTrackBar.Value).ToString();
        }

        private void LeadershipTrackBar_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            e.ToolTipText = "Indicates a soldier's ability to lead (Communication, Discipline)";
        }

        private void AdaptTrackBar_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            e.ToolTipText = "";
        }

        private void StabilityTrackBar_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            e.ToolTipText = "Indicates a soldier's..";
        }

        private void MarksmanTrackBar_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            e.ToolTipText = "Indicates a soldier's marksmanship ability (weapon handling skills)";
        }

        private void FitnessTrackBar_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            e.ToolTipText = "Indicates a soldier's fitness level and overall military bearing.";
        }

        private void TeamWorkTrackBar_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            e.ToolTipText = "A person with a high level of agreeableness in a personality test is usually warm, friendly, and tactful. A person who scores low on agreeableness may put their own interests above those of others. They tend to be distant, unfriendly, and uncooperative.";
        }

        private void AmbitionTrackBar_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            e.ToolTipText = "";
        }

        private void ExtraversionTrackBar_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            e.ToolTipText = "";
        }

        private void ConTrackBar_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            e.ToolTipText = "";
        }

        private void NeuroticismTrackBar_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            e.ToolTipText = "";
        }

        private void IntellectTrackBar_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            e.ToolTipText = "";
        }

        private void ImproveTrackBar_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            e.ToolTipText = "";
        }

        #endregion Labels

        private void PersonaEditorForm_Load(object sender, EventArgs e)
        {
            ActiveControl = null;
            ProbabilityTrackBar.Focus();
        }

        private void ageSkewLeftTrackBar_ValueChanged(object sender, EventArgs e)
        {
            ageSkewLeftValueLabel.Text = ageSkewLeftTrackBar.Value.ToString();
        }

        private void ageSkewRightTrackBar_ValueChanged(object sender, EventArgs e)
        {
            ageSkewRightValueLabel.Text = ageSkewRightTrackBar.Value.ToString();
        }

        private void careerSkewLeftTrackBar_ValueChanged(object sender, EventArgs e)
        {
            careerSkewLeftValueLabel.Text = careerSkewLeftTrackBar.Value.ToString();
        }

        private void careerSkewRightTrackBar_ValueChanged(object sender, EventArgs e)
        {
            careerSkewRightValueLabel.Text = careerSkewRightTrackBar.Value.ToString();
        }
    }
}
