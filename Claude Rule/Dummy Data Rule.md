# Dummy Data Generation — Complete Reference

## Overview

Dummy data is generated in the C# WPF application (`FrmDummyData`) to test survey script logic and routing before live fieldwork. The engine reads a **script database** (questionnaire `.db`), generates realistic random answers for every question, and writes them into a copy of the **answer database** template (`SYSACDB.db`).

---

## Databases Involved

| Database | File | Role |
|---|---|---|
| Script / Questionnaire DB | `*.db` (e.g. `SU8A9XRN.db`) | Read-only — contains questions, attributes, logic |
| Answer DB template | `ShellDB\SYSACDB.db` | Copied once per run as the output file |
| Answer DB output | `DummyData\DummyData_{timestamp}.db` | Written — holds all generated responses |

### Answer DB Tables Written

#### `T_InterviewInfo` — one row per respondent
| Column | Type | Dummy Value |
|---|---|---|
| ProjectId | INT | From `T_ProjectInfo.ProjectId` |
| RespondentId | BIGINT | Timestamp + zero-padded index (e.g. `202605130930000001`) |
| Latitude | VARCHAR | Random within Bangladesh: 20.5 – 26.6 |
| Longitude | VARCHAR | Random within Bangladesh: 88.0 – 92.7 |
| SurveyDateTime | DATETIME | System time at start of respondent loop |
| SurveyEndTime | DATETIME | System time after all answers saved |
| LengthOfIntv | VARCHAR | Random 15 – 50 (minutes) |
| Intv_Type | VARCHAR | `"1"` (Complete) |
| FICode | VARCHAR | Random from pool: FI001 – FI005 |
| FSCode | VARCHAR | Random from pool: FS001 – FS003 |
| Status | VARCHAR | `"2"` (Dummy / Incomplete) |
| ScriptVersion | VARCHAR | From `T_ProjectInfo.Version` |
| LanguageId | VARCHAR | `"1"` (English) |

#### `T_RespAnswer` — one or more rows per question per respondent
| Column | Description |
|---|---|
| ProjectId | Project identifier |
| RespondentId | Links to T_InterviewInfo |
| QId | Question ID (or sub-question ID for grid/form: `QId_AttrValue`) |
| Response | The answer value (attribute code, number, text, or date) |
| ResponseDateTime | System datetime |
| qElapsedTime | Random 2 – 25 seconds |
| qOrderTag | `OrderTag1` of the question |
| rOrderTag | `AttributeOrder` from `T_OptAttribute` for the saved attribute; falls back to `1` when no attribute exists for the question |

#### `T_RespOpenended` — only when selected attribute has `TakeOpenended = '1'`
A row is written here **only** when the attribute that was selected and saved to `T_RespAnswer` has `TakeOpenended = '1'` in `T_OptAttribute`. If the attribute has any other value (0, null, blank), nothing is written to this table — the answer stays in `T_RespAnswer` only.

| Column | Description |
|---|---|
| ProjectId | Project identifier |
| RespondentId | Links to T_InterviewInfo |
| QId | Question ID |
| AttributeValue | `AttributeValue` of the attribute that has `TakeOpenended = '1'` |
| OpenendedResp | Generated text: `"Sample OE response for {QId}"` |
| OEResponseType | `"1"` (verbatim) |

#### `T_IntvSettings` — one row per respondent (sampling / locality data)
| Column | Dummy Value |
|---|---|
| ProjectId | From `T_ProjectInfo.ProjectId` |
| RespondentId | Links to T_InterviewInfo |
| QId | `""` (empty — not applicable for dummy runs) |
| LocalityName | Random from 10-item pool (Dhaka North, Chittagong, Sylhet, etc.) |
| SegmentNo | Random 1 – 10 |
| MappersId | Random from pool: MP001 – MP009 |
| Segment1 – Segment10 | Each random 1 – 50 |
| RNumber | Random 1 – 99 |
| SelectedSeg | Random 1 – 10 |

#### `T_RespAnsLog` — one row per answered question per respondent (timing log)
| Column | Dummy Value |
|---|---|
| ProjectId | From `T_ProjectInfo.ProjectId` |
| RespondentId | Links to T_InterviewInfo |
| QId | The question ID that was answered |
| qElapsedTime | Random 2 – 25 seconds |
| ResponseDateTime | System datetime at time of answer |

> **Note:** A row is only written to `T_RespAnsLog` when `GenerateAndSaveAnswer` returns `saved > 0` (i.e. SKIP/STOP types are excluded).

---

## Script Database Tables Read

| Table | Purpose |
|---|---|
| `T_ProjectInfo` | ProjectId, ProjectName, Version |
| `T_Question` | All questions ordered by `OrderTag1` |
| `T_OptAttribute` | Answer options / sub-fields for each question |
| `T_GridInfo` | Named option sets used by grid and form questions |
| `T_LogicTable` | Jump/skip routing rules (LogicTypeId = 3) |

---

## Question Walker Logic

1. Load all questions from `T_Question WHERE ProjectId=? ORDER BY CAST(OrderTag1 AS INTEGER)` into memory once.
2. Walk from `OrderTag1 = 1` upward until the maximum order or a STOP type is reached.
3. For each question:
   - If **STOP type** (50, 51) → break the loop.
   - If **SKIP type** (6, 9, 11, 16, 49) → advance to next order, write nothing.
   - Otherwise → call the appropriate QType handler → save answers → evaluate routing.
4. **Routing (optional)**: query `T_LogicTable WHERE QId=? AND LogicTypeId='3'`, evaluate `IfCondition` using `CheckCondition`, jump to `Then` or `Else` target question's `OrderTag1`.
5. Safety limit: maximum iterations = `maxOrderTag + 200` to prevent infinite loops.

### Transaction Strategy
| Routing mode | Transaction |
|---|---|
| Routing OFF (default) | One transaction per respondent — fast |
| Routing ON | Autocommit per insert — slower, required so `CheckCondition` can read prior answers via a separate connection |

---

## HasAutoResponse = '2'

Questions where `HasAutoResponse = '2'` are system-derived (auto-filled by the app from prior answers via `T_LogicAuto`). In dummy data generation these are **treated the same as regular questions** — a random valid value is generated from their `T_OptAttribute` entries. This ensures full data coverage even if the derived value may not be logically consistent with prior answers.

---

## All QType Rules

### Group 1 — SKIP (no rows written)
| QType | Name | Rule |
|---|---|---|
| 6 | Image | Display only — skip entirely |
| 9 | MediaPlayer | Display only — skip entirely |
| 11 | PowerPointSlide | Display only — skip entirely |
| 16 | CaptureImage | Device camera only — skip entirely |
| 49 | No Control | Instruction / display screen — skip entirely |

### Group 2 — STOP (end walker)
| QType | Name | Rule |
|---|---|---|
| 50 | Thank You | Break the question loop |
| 51 | Terminate Thank You | Break the question loop |

### Group 3 — Single Coded Value
One row in `T_RespAnswer` with `rOrderTag = 1`.

| QType | Name | Rule |
|---|---|---|
| 1 | Single Response | Pick 1 random `AttributeValue` from `T_OptAttribute` |
| 24 | Spinner (dropdown) | Same as QType 1 |
| 61 | Scale 10 Table View (SR) | Same as QType 1 |

### Group 4 — Multiple Coded Values
Multiple rows in `T_RespAnswer`, one per selected option, `rOrderTag = 1, 2, 3…`

#### QType 2 — Multiple Response
- Read `NoOfResponseMin` and `NoOfResponseMax` from `T_Question`.
- Default: min = 1, max = all attributes if blank.
- Separate attributes into **exclusive** (`IsExclusive = '1'`) and **non-exclusive**.
- 20% chance to select one exclusive attribute (and no others).
- Otherwise pick `rand(min, max)` non-exclusive attributes randomly.
- Each selected attribute → one row: `Response = AttributeValue`, `rOrderTag = position (1, 2, 3…)`.

#### QType 5 — Rank
- Read `NoOfResponseMax` for how many items to rank (default = all attributes).
- Shuffle all attributes randomly.
- Take top `NoOfResponseMax` items.
- Save each as: `Response = AttributeValue`, `rOrderTag = rank position (1 = top)`.

#### QType 26 — Drag Drop
- Identical logic to QType 5 (Rank).

### Group 5 — Open-Ended Text

#### QType 3 — Openended String
- **30% chance** → pick the exclusive attribute (DK/CS): write `Response = AttributeValue` to `T_RespAnswer` with `rOrderTag = AttributeOrder` of that attribute.
- **70% chance** → pick the non-exclusive (OE) attribute: write `Response = AttributeValue` to `T_RespAnswer` with `rOrderTag = AttributeOrder` of that attribute.
  - If the selected attribute has `TakeOpenended = '1'`: also write `"Sample OE response for {QId}"` to `T_RespOpenended`.
  - Otherwise: nothing written to `T_RespOpenended`.
- If no attributes exist: write `Response = '1'` to `T_RespAnswer` with `rOrderTag = 1`. Nothing written to `T_RespOpenended`.

#### QType 18 — Openended String With DKCS
- Same logic as QType 3.

#### QType 10 — SoundRecorder
- No text answer possible in dummy data.
- Write `Response = 'dummy_audio.m4a'` to `T_RespAnswer` as a placeholder.

### Group 6 — Open-Ended Numeric

#### QType 4 — Openended Number
- Look for a non-exclusive attribute in `T_OptAttribute` for `MinValue` / `MaxValue`.
- If no attribute: use `NoOfResponseMin` / `NoOfResponseMax` from `T_Question`.
- If still blank: default range = 1 – 99.
- **20% chance DK** if a DK exclusive attribute exists.
- Otherwise: write `Response = random int in [min, max]` to `T_RespAnswer`.

#### QType 19 — Openended Number With DKCS
- Same logic as QType 4.

#### QType 25 — Slider Control
- Read `MinValue` / `MaxValue` from first attribute in `T_OptAttribute`.
- Default: 0 – 100.
- Write `Response = random int in [min, max]` to `T_RespAnswer`.

### Group 7 — Date / Time

#### QType 14 — DateControl
- Generate a random date between today and 2 years ago.
- Write `Response = "YYYY-MM-DD"` to `T_RespAnswer`.

#### QType 15 — TimeControl
- Generate a random time between 08:00 and 19:59.
- Write `Response = "HH:MM"` to `T_RespAnswer`.

### Group 8 — List Open-Ended

#### QType 12 — List Openended String
- One row per attribute in `T_RespAnswer`.
- `Response = "Sample text for {QId} option {AttributeValue}"`.
- `rOrderTag = AttributeOrder` of each attribute.

#### QType 13 — List Openended Number
- One row per attribute in `T_RespAnswer`.
- `Response = random int` within that attribute's `MinValue` – `MaxValue`.
- Default range: 0 – 99 if blank.
- `rOrderTag = AttributeOrder`.

#### QType 17 — List Openended Number With Total
- Same as QType 13 for all **non-exclusive** attributes.
- The **exclusive** attribute (`IsExclusive = '1'`) is the TOTAL row.
- `Response` for the total = **sum of all non-exclusive values**.
- All rows saved to `T_RespAnswer`.

### Group 9 — Grid (Rows × Column Options)
For each row attribute, the column options come from `T_GridInfo WHERE QId = LinkId2`.
Sub-question ID format: `{MainQId}_{RowAttributeValue}` (e.g. `AD5_1` for row 1).

#### QType 7 — GridOption (single-select per row)
- For each row attribute in `T_OptAttribute`:
  - Look up column options from `T_GridInfo` using `LinkId2`.
  - Pick **exactly 1** column option randomly.
  - Write `QId = "{MainQId}_{RowAttrValue}"`, `Response = column AttributeValue`, `rOrderTag = 1`.

#### QType 8 — GridCheckBox (multi-select per row)
- Same row structure as QType 7.
- For each row: pick **1 to N** column options randomly (N = all columns available).
- Write one `T_RespAnswer` row per selected column, `rOrderTag = 1, 2, 3…`

#### QType 27 — GridNumber
- For each row attribute in `T_OptAttribute`:
  - Generate a random integer within that attribute's `MinValue` – `MaxValue`.
  - Default range: 0 – 100 if blank.
  - Write `QId = "{MainQId}_{RowAttrValue}"`, `Response = number`, `rOrderTag = 1`.

### Group 10 — Scale Grid (Rows × Scale Rating)
For each row attribute, generate a single scale integer.
Sub-question ID format: `{MainQId}_{RowAttributeValue}`.

| QType | Name | Scale | Rule |
|---|---|---|---|
| 31 | Scale 5 Table View | 1 – 5 | Random int 1–5 per row; write `QId = "{QId}_{RowAttrValue}"`, `rOrderTag = 1` |
| 32 | Scale 7 Table View | 1 – 7 | Random int 1–7 per row |
| 33 | Scale 10 Table View | 1 – 10 | Random int 1–10 per row |

### Group 11 — Form / Compound Questions
Each attribute in `T_OptAttribute` is a **sub-field** with its own data type defined by `LinkId1`.
Sub-question ID format: `{FormQId}_{AttributeValue}` (e.g. `MemInfo1_1` for Name field).

| LinkId1 value | Sub-field type | Dummy data rule |
|---|---|---|
| `1` | Single Response | Look up options from `T_GridInfo WHERE QId = LinkId2`; pick 1 randomly |
| `3` | OE String | Generate `"Sample_{FormQId}_{AttributeValue}"` |
| `4` | OE Number | Random int within attribute's `MinValue` – `MaxValue`; default 1–99 |
| other | Unknown | Generate `"Value_{AttributeValue}"` |

Each sub-field → one row in `T_RespAnswer` with `rOrderTag = 1`.

| QType | Name | Notes |
|---|---|---|
| 48 | Form | General compound form |
| 20 | Member Information | Household member data (same compound structure) |
| 21 | Kids Information | Children's data (same compound structure) |
| 60 | FIFS Info | All sub-fields are `LinkId1 = '3'` (string); generates FI/FS name/code values |

### Group 12 — Auto-Suggestion

#### QType 22 — Auto Suggestion
- Text autocomplete input.
- Write `Response = "AutoSug_{QId}_{random 3-digit number}"` to `T_RespAnswer`.

#### QType 23 — Auto Suggestion (List from Response)
- Same rule as QType 22.
- Note: in a live app this would pull from a prior question's response list; in dummy data a generated text value is sufficient.

### Group 13 — GPS

#### QType 41 — GetGPS
- Two rows written to `T_RespAnswer` for the same `QId`:
  - Row 1: `Response = latitude` (random 6 d.p. in range 20.5 – 26.6), `rOrderTag = 1`
  - Row 2: `Response = longitude` (random 6 d.p. in range 88.0 – 92.7), `rOrderTag = 2`
- Bounding box represents Bangladesh geographic extent.

### Group 14 — MaxDiff (Best-Worst Scaling)

#### QType 40 — Maxdiff
- `NoOfResponseMax` on `T_Question` = number of rounds; default = `max(1, attr_count / 4)`.
- Per round (qOrderTag = round number):
  - Take a random subset of 3 – 5 attributes from `T_OptAttribute`.
  - **Best**: first item → `Response = AttributeValue`, `rOrderTag = 1`.
  - **Worst**: second item → `Response = AttributeValue`, `rOrderTag = 2`.
- Each round writes 2 rows to `T_RespAnswer`.

---

## T_GridInfo — Named Option Sets

`T_GridInfo` stores reusable option sets referenced by `LinkId2` in `T_OptAttribute`.

| QId (set name) | Options |
|---|---|
| `Gender` | Male (1), Female (2) |
| `YesNo` | Yes (1), No (2) |

Any new named sets added in future scripts are automatically supported — the engine queries `T_GridInfo WHERE QId = LinkId2` dynamically.

---

## Routing Logic (T_LogicTable)

| Column | Description |
|---|---|
| QId | The question the rule is attached to |
| LogicTypeId | `'3'` = Jump/Skip (used for routing) |
| IfCondition | Expression evaluated after answering the question |
| Then | Target QId if condition is TRUE |
| Else | Target QId if condition is FALSE (blank = next question) |

- Routing is **optional** — controlled by the "Follow Routing Logic" checkbox.
- When enabled, `CheckCondition.convetToPostFixNotationAndExecute()` evaluates the `IfCondition` expression against already-saved answers in `T_RespAnswer`.
- The engine looks up `OrderTag1` of the `Then` / `Else` target and jumps to that index.
- If condition evaluation fails (exception), routing defaults to sequential (next question).

---

## Sub-Question ID Naming Convention

| Pattern | Used by |
|---|---|
| `{QId}_{RowAttributeValue}` | QType 7, 8, 27, 31, 32, 33 (grid/scale rows) |
| `{FormQId}_{AttributeValue}` | QType 20, 21, 48, 60 (form sub-fields) |

Examples:
- `AD5_1` = Grid question `AD5`, row for Grameenphone (AttributeValue = 1)
- `MemInfo1_2` = Form question `MemInfo1`, Age sub-field (AttributeValue = 2)

---

## Output Files

Each run creates a new timestamped file — **previous runs are never overwritten**.

```
[ScriptFolder]\
└── DummyData\
    ├── DummyData_20260513_093045.db   ← run 1
    ├── DummyData_20260513_094512.db   ← run 2
    └── ...
```

---

## Key Implementation Files

| File | Purpose |
|---|---|
| `Forms\Scripting\FrmDummyData.xaml` | UI — browse, number of records, progress, log |
| `Forms\Scripting\FrmDummyData.xaml.cs` | Engine — all QType handlers, walker, routing |
| `ShellDB\SYSACDB.db` | Answer DB template (must contain T_InterviewInfo, T_RespAnswer, T_RespOpenended) |
| `Classes\CheckCondition.cs` | Condition parser used by routing logic |
| `Classes\ConnectionDB.cs` | SQLite connection wrapper |
| `Classes\DBHelper.cs` | Query helper (uses StaticClass.QDBPath / ADBPath) |
| `Classes\StaticClass.cs` | Global paths: QDBPath, ADBPath |

---

## Quick Reference — QType to Action

| QType | Name | T_RespAnswer rows | T_RespOpenended rows |
|---|---|---|---|
| 1 | Single Response | 1 | — |
| 2 | Multiple Response | min–max | — |
| 3 | OE String | 1 | 0 or 1 (only if selected attr has `TakeOpenended='1'`) |
| 4 | OE Number | 1 | — |
| 5 | Rank | N (ranked) | — |
| 6 | Image | **SKIP** | — |
| 7 | GridOption | 1 per row | — |
| 8 | GridCheckBox | 1–N per row | — |
| 9 | MediaPlayer | **SKIP** | — |
| 10 | SoundRecorder | 1 (filename) | — |
| 11 | PowerPointSlide | **SKIP** | — |
| 12 | List OE String | 1 per attribute | — |
| 13 | List OE Number | 1 per attribute | — |
| 14 | DateControl | 1 (YYYY-MM-DD) | — |
| 15 | TimeControl | 1 (HH:MM) | — |
| 16 | CaptureImage | **SKIP** | — |
| 17 | List OE Number With Total | 1 per attr + 1 total | — |
| 18 | OE String With DKCS | 1 | 0 or 1 (only if selected attr has `TakeOpenended='1'`) |
| 19 | OE Number With DKCS | 1 | — |
| 20 | Member Information | 1 per sub-field | — |
| 21 | Kids Information | 1 per sub-field | — |
| 22 | Auto Suggestion | 1 (text) | — |
| 23 | Auto Suggestion (from Response) | 1 (text) | — |
| 24 | Spinner (dropdown) | 1 | — |
| 25 | Slider Control | 1 | — |
| 26 | Drag Drop | N (ranked) | — |
| 27 | GridNumber | 1 per row | — |
| 31 | Scale 5 Table View | 1 per row (1–5) | — |
| 32 | Scale 7 Table View | 1 per row (1–7) | — |
| 33 | Scale 10 Table View | 1 per row (1–10) | — |
| 40 | Maxdiff | 2 per round | — |
| 41 | GetGPS | 2 (lat + lon) | — |
| 48 | Form | 1 per sub-field | per str sub-field |
| 49 | No Control | **SKIP** | — |
| 50 | Thank You | **STOP** | — |
| 51 | Terminate Thank You | **STOP** | — |
| 60 | FIFS Info | 1 per sub-field | — |
| 61 | Scale10TableView (SR) | 1 | — |
