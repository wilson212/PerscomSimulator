using Google.GenAI;
using Google.GenAI.Types;
using Perscom.AI;
using System;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.ConversationalUI;
using Telerik.WinControls.VirtualKeyboard;
using Telerik.Windows.Diagrams.Core;

namespace Perscom
{
    /// <summary>
    /// Represents a chat form utilized for interaction between the user and the advisor component.
    /// Inherits functionality from RadForm and supports singleton behavior for managing a single
    /// instance of the form throughout the application's lifecycle.
    /// </summary>
    public partial class AdvisorChatForm : RadForm
    {
        /// <summary>
        /// The author of the AI's messages.'
        /// </summary>
        private Author AiAuthor { get; set; }

        /// <summary>
        /// Represents the user participating in the chat session.
        /// </summary>
        private Author User { get; set; }

        /// <summary>
        /// The advisor model instance used for generating responses.
        /// </summary>
        private Advisor Model { get; set; }

        private static int SelectedFactionId { get; set; }

        private static AdvisorChatForm _instance;
        private static string _apiKey;
        private static string _modelId;

        /// <summary>
        /// Private constructor for singleton pattern.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="modelId"></param>
        private AdvisorChatForm(string apiKey, string modelId)
        {
            // Create components and apply theme
            InitializeComponent();
            FormStyling.ApplyControlsTheme(Controls);

            Model = new Advisor(apiKey, modelId, () => SelectedFactionId);
            AiAuthor = new Author("Virtual Assistant");
            User = new Author("You");

            chatWindow.Author = User;

            // Add Settings
            ToolbarActionDataItem settingsAction = new ToolbarActionDataItem(Properties.Resources.settings, "settings");
            chatWindow.ChatElement.ToolbarElement.AddToolbarAction(settingsAction);

            // Start collapsed
            chatWindow.ChatElement.ToolbarElement.Visibility = ElementVisibility.Collapsed;
        }

        /// <summary>
        /// Call once at startup to configure the API key and model.
        /// </summary>
        public static void Configure(string apiKey, string modelId)
        {
            _apiKey = apiKey;
            _modelId = modelId;
        }


        public static void SetFactionId(int id)
        {
            SelectedFactionId = id;

            // Todo Need to update the Advisor if the form is open, so it knows??
        }

        /// <summary>
        /// Shows the singleton instance. Creates it if needed, or brings it to front if already open.
        /// </summary>
        public static void Open(RadForm parent = null, bool asDialog = false)
        {
            // If no API key configured, prompt the user first
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                using (var configForm = new AdvisorConfigForm())
                {
                    if (parent != null)
                        configForm.StartPosition = FormStartPosition.CenterParent;

                    if (configForm.ShowDialog(parent) != DialogResult.OK)
                        return; // User cancelled, don't open chat

                    // Pull the configured values from the config form
                    _apiKey = configForm.ApiKey;
                    _modelId = configForm.SelectedModel;
                }
            }

            // Ensure we have an instance
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new AdvisorChatForm(_apiKey, _modelId);
            }

            //  Set parent
            if (parent != null)
            {
                _instance.Owner = parent;
            }

            // Show if not visible
            if (!_instance.Visible)
            {
                if (asDialog)
                {
                    _instance.StartPosition = FormStartPosition.CenterParent;
                    _instance.ShowDialog();
                }
                else
                {
                    // Get the working area of the screen the parent is on
                    var screen = Screen.FromControl(parent);
                    var workingArea = screen.WorkingArea;

                    // Try placing to the right of the parent
                    int x = parent.Right;
                    int y = parent.Top;

                    // If it would overflow the right edge, place to the left instead
                    if (x + _instance.Width > workingArea.Right)
                        x = parent.Left - _instance.Width;

                    // Clamp Y so it doesn't go off-screen vertically
                    if (y + _instance.Height > workingArea.Bottom)
                        y = workingArea.Bottom - _instance.Height;
                    if (y < workingArea.Top)
                        y = workingArea.Top;

                    _instance.StartPosition = FormStartPosition.Manual;
                    _instance.Location = new System.Drawing.Point(x, y);
                    _instance.Show();
                }
            }
            else
            {
                _instance.BringToFront();
            }
        }

        /// <summary>
        /// Closes the singleton instance.
        /// </summary>
        public static new void Close()
        {
            if (_instance != null && !_instance.IsDisposed)
            {
                _instance.Hide();
            }
        }

        /// <summary>
        /// Fully destroys the singleton instance and releases resources.
        /// </summary>
        public static void Destroy()
        {
            if (_instance != null && !_instance.IsDisposed)
            {
                ((RadForm)_instance).Close();
                _instance.Dispose();
            }
            _instance = null;
        }

        // Override OnFormClosing to hide instead of destroy, preserving chat history
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                return;
            }
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Handles the load event of the AdvisorChatForm.
        /// Initializes the chat window with a welcome message from the AI advisor.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void AdvisorChatForm_Load(object sender, EventArgs e)
        {
            if (chatWindow.ChatElement.MessagesViewElement.Items.Count == 0)
            {
                AIChatTextMessage message = new AIChatTextMessage(
                    "Advisor online. I am ready to assist with TO&E generation, rank structures, and unit blueprints. " +
                    "You can ask me to draft new elements from scratch, or attach an existing unit from your database " +
                    "for me to review and staff. What are we standing up today?",
                    AiAuthor,
                    DateTime.Now
                );
                chatWindow.AddMessage(message);
            }
        }

        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {
            FormStyling.StyleFormHeader(headerPanel, e);
            base.OnPaint(e);
        }

        /// <summary>
        /// Handles the SendMessage event of the chatWindow.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void chatWindow_SendMessage(object sender, SendMessageEventArgs e)
        {
            // Lock the chat window
            chatWindow.ChatElement.InputTextBox.Enabled = false;

            // Show that the AI is typing
            chatWindow.ChatElement.ShowTypingIndicator(AiAuthor);

            // Check user message
            ChatTextMessage textMessage = e.Message as ChatTextMessage;
            if (String.IsNullOrWhiteSpace(textMessage?.Message))
            {
                // Unlock the chat window
                chatWindow.ChatElement.InputTextBox.Enabled = true;
                return;
            }

            string response;
            try
            {
                // Send the user message
                response = await Model.SendMessageAsync(textMessage.Message, null, chatWindow, AiAuthor);
            }
            catch (Exception ex)
            {
                response = "⚠ An unhandled Exception occured: " + ex.Message;
                ExceptionHandler.GenerateExceptionLog(ex);
            }

            // Await the response
            AIChatTextMessage message = new AIChatTextMessage(response, AiAuthor, DateTime.Now);
            chatWindow.AddMessage(message);
            chatWindow.Refresh();

            // Close typing indicater
            chatWindow.ChatElement.HideTypingIndicator();

            // Unlock the chat window
            chatWindow.ChatElement.InputTextBox.Enabled = true;
            chatWindow.ChatElement.InputTextBox.Focus();
        }

        private void chatWindow_ToolbarActionClicked(object sender, ToolbarActionEventArgs e)
        {
            ToolbarActionDataItem action = e.DataItem;
            switch (action.UserData.ToString())
            {
                case "settings":
                    using (var configForm = new AdvisorConfigForm())
                    {
                        configForm.StartPosition = FormStartPosition.CenterParent;

                        if (configForm.ShowDialog(_instance) != DialogResult.OK)
                            return; // User cancelled, don't open chat

                        // Pull the configured values from the config form
                        _apiKey = configForm.ApiKey;
                        _modelId = configForm.SelectedModel;

                        // Clear chat window and create a new Model
                        Model = new Advisor(_apiKey, _modelId, () => SelectedFactionId);
                        chatWindow.ChatElement.MessagesViewElement.Items.RemoveLast();

                        // Reload
                        AIChatTextMessage message = new AIChatTextMessage(
                            "Your API Key and Model preferences have been updated!",
                            AiAuthor,
                            DateTime.Now
                        );
                        chatWindow.AddMessage(message);

                        // Ready the user
                        chatWindow.ChatElement.ToolbarElement.Visibility = ElementVisibility.Collapsed;
                        chatWindow.ChatElement.InputTextBox.Focus();
                        chatWindow.Refresh();
                    }
                    return;
            }
        }
    }
}
