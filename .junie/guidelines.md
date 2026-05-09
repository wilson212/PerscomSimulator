# Project Overview: Perscom Simulator

## 1. High-Level Goal

Perscom is a highly realistic C# WinForms application designed as an analytical cause-and-effect sandbox for a massive MILSIM paintball league.
It allows the user to define organizational structures, rank hierarchies, promotion boards, and soldier personas.

The application simulates military personnel promotions, lateral movements, retirements, and career trajectories over decades.
The ultimate goal is to reach a mathematically pure steady-state, allowing the user to observe the exact statistical
outcomes (e.g., average Time in Grade, retention rates) caused by their specific ruleset.

## 2. Core Architecture & Strict AI Rules

- **UI Framework:** Telerik UI for WinForms. **Note:** I prefer using standard WinForms `Panels` for layout over `RadPanels` due to styling.
- **Database:** SQLite. All data is persisted in a single SQLite database file.
- **Data Access:** A custom-built micro-ORM named **CrossLite**. All data entities are located in the `/Database/Entities/` folder.

## 3. Domain Language & Key Concepts

### Blueprints vs. Instances vs. Wrappers:

- UnitBlueprint, PositionBlueprint, of any "Blueprint" class act as stateless templates, that define rules.
- Unit, Position, and Soldier are the stateful, unique instances spawned from those blueprints.
- Echelon: The hierarchical level of a unit (e.g., Fire Team, Squad, Platoon, Company, Battalion, Division, Faction).
- Classes suffixed with Wrapper (e.g., SoldierWrapper, UnitWrapper, PromotionBoardWrapper) act as the active runtime controllers. They bridge the gap between the raw SQLite database entities (which are strictly handled by the CrossLite ORM) and the live simulation. While database entities only hold persistent state data, Wrappers hold the complex simulation logic, event subscriptions, and transient in-memory caching needed to execute the simulation loop efficiently.
- Time in Grade (TIG): Months spent at the current rank.
- Time in Service (TIS): Total months in the league.
- Form Rating: A dynamically calculated score (1.0 to 9.9) measuring a soldier's recent performance momentum, heavily influenced by their Morale and how their skills align with their position's expected weights.
- Flight Risk: A dynamic 0-100 score calculated monthly that determines if a soldier will quit the league. It is influenced by financial strain (subscription costs vs. rank stipends), innate traits, low Morale, and their TargetTIS (a hidden bell-curve value representing their intended hobby lifespan, rather than a hardcoded expiration date).

## 4. Soldier Data Structure

- Soldiers are highly dynamic entities governed by RPG-style attributes:
- Innate Mental Attributes (Static Multipliers): Adaptability, Intelligence, Improvability.
- Innate Personality Attributes (Static Behaviors): Agreeableness, Ambition, Conscientiousness, Extraversion, Mindfulness, Courage.
- Basic Skills (Dynamic, grow over time): Leadership, Marksmanship, Fitness, Teamwork, Composure, Discipline.
- Traits: Randomly assigned modifiers (e.g., Born Leader, Slacker, Content) that mathematically alter the underlying attributes and influence the AI's career decisions.
- Morale: A highly volatile 0-100 multiplier affected by evaluation boards, squad cohesion, leadership quality, and Time in Grade stagnation.
- *Important:* All attributes are in a range of 0-20.

## 5. Promotion Board Architecture

- The simulation supports both fluid pay-grade promotions and strict career tracks (e.g., USMC First Sergeant vs. Master Sergeant) through hierarchical database scoping.
- The Ruleset (PromotionBoard): Database entity defining weights, pass thresholds, and scopes. It checks for specific OccupationId (specialty tracks), then falls back to RankId (strict rank tracks), and finally falls back to RankClassificationId (generic pay-grade boards).
- The Mock Board (Career Path Selection): When faced with a branching rank structure, the soldier's AI projects their score across all available boards. They lock in their TargetRankId based on the path that mathematically favors their innate stats.
- The Instance (UnitPromotionBoard): Boards are stateful and scoped to specific Echelons (e.g., a Battalion-level board).
- The Standing List (PromotableCandidate): Soldiers do not get evaluated at the moment a vacancy opens. They periodically submit packets, get evaluated, and if they pass, receive a "P" (Promotable) status, generating a single PromotableCandidate record pointing to their TargetRankId.

## 6. The Simulation Loop (The 5 Phases)

The core simulation runs month-by-month through a strict, multi-phase pipeline to prevent logic overlaps and ensure O(1) efficiency during vacancy filling.

### Phase 1: Time & State Update (Soldier-Centric)
- Increment TIS/TIG.
- Apply monthly skill growth (modified by Improvability).
- Recalculate Morale and Flight Risk.

### Phase 2: Attrition

- Evaluate FlightRiskScore and MaxTourLength burnout.
- Process retirements and remove soldiers from the active roster. This naturally creates all necessary vacuums/vacancies for Phase 4.

### Phase 3: The Boards Convene

- Soldiers evaluate options for the next grade. If branching paths exist, the soldier runs a "Mock Board" and permanently locks in their TargetRankId.
- Soldiers submit packets to their scoped UnitPromotionBoard (walking up their Chain of Command to find the correct Echelon level).
- The board scores them based on Basic Skills, Form Rating, and TimeInGradeFactor. Winners are granted "P" status.

### Phase 4: The Waterfall (Vacancy Filling)

- Iterates top-down through all Positions ordered by stature (Generals down to Privates).
- Priority 1: Lateral Transfers. Moving a stagnant soldier who has exceeded their MaxTourLength to refresh their Morale.
- Priority 2: Promotions. The open position checks its required PromotionPoolLevel (Echelon), walks up the chain of command to that HQ, and scoops the top-scoring "P" status candidate from the in-memory registry.

### Phase 5: Entry-Level Recruitment

- Brand new Soldier entities are spawned from SoldierBlueprints to fill bottom-tier entry vacancies.
- Database changes are committed via CrossLite.

## 7. Crucial Performance Patterns

The Headless Burn-In ("Ghost Run"): To avoid Day-Zero statistical corruption, the simulation can run entirely in RAM for X years (e.g., 20 years) without triggering CrossLite database writes or WinForms UI updates.
Once the population naturally stabilizes into a steady state, the final snapshot is bulk-inserted into SQLite as "Month 0".

Event-Driven Memory Registry: To avoid O(N) tree searches when filling vacancies, the UnitWrapper maintains an in-memory dictionary of promotable candidates (PromotablePool).
Global C# events (OnPositionChange, OnRetire) automatically register and deregister pointers up the chain of command, ensuring the promotion lists are perfectly synced and
lightning-fast to query, with zero database duplication.

### Data Access (MOST IMPORTANT)

- All database read/write operations **MUST** be performed using the `CrossLite` Unit of Work pattern. There are no exceptions.
