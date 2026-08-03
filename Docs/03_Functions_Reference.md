# SmartSurvey Script — Functions & Methods Reference

> **Scope:** This document describes all major private methods inside `FrmBuildScript.xaml.cs`
> (the script compiler). It covers what each function does, its inputs, outputs, and how it
> connects to the rest of the build pipeline.

---

## Architecture Overview

```
btnExecute_Click()
    │
    ├── ReadLanguageSection()           — reads @LANGUAGE sections
    │
    ├── [English section loop]
    │     ├── prepareList()             — *LIST definitions
    │     ├── prepareGridList()         — *GRIDLIST definitions
    │     ├── prepareIf()              — *IF logic blocks
    │     ├── prepareIncludeExclude()  — standalone *INCLUDE/*EXCLUDE
    │     ├── prepareQuestion()        — *QUESTION parsing (main)
    │     └── ExpandRepeatBlockEnglish() — *REPEAT expansion
    │
    ├── [Language 1-9 loops]
    │     ├── prepareListForLanguage()
    │     ├── prepareGridListForLanguage()
    │     ├── prepareQuestionForLanguage()
    │     └── ExpandRepeatBlockLanguage()
    │
    ├── BuildRepeatIterationList()      — used by REPEAT blocks
    │
    ├── getShellDB()                    — loads template .db file
    │
    └── [SQL INSERT loop]               — writes all data to SQLite
          ├── T_ProjectInfo
          ├── T_Question
          ├── T_OptAttribute
          ├── T_GridInfo
          ├── T_LogicTable
          ├── T_LogicAuto
          └── T_OptAttrbFilter
```

---

## UI & Entry Points

---

### `btnBrowse_Click`

**Purpose:** Opens a file dialog to select the `.q` script file.

**Behaviour:**
- Opens `OpenFileDialog` filtered to `*.q` files
- Sets `txtScriptPath.Text` to the selected file path
- Saves the directory to `Properties.Settings.Default.StartupPath` for next session
- Clears any previous build output

**No output — only sets UI state.**

---

### `btnExecute_Click`

**Purpose:** Main build entry point. Orchestrates the entire script-to-database compilation.

**Flow:**
1. Clears UI and sets wait cursor
2. Initialises all dictionaries and lists (questions, attributes, grid lists, etc.)
3. Reads the `.q` file line by line, splitting at `@LANGUAGE` markers
4. Parses the **English section** — questions, lists, IF logic
5. Parses **Language 1–9 sections** (if present)
6. Validates mandatory QIds (`RespName`, `RespMobile`, `Centre`, `FIFSInfo`)
7. Calls `getShellDB()` to copy the shell database template
8. Opens a SQLite connection and begins a transaction
9. Inserts all parsed data into T_* tables
10. Commits transaction; reports success or error count

**Output:** `<scriptname>.db` SQLite database in the same folder as the `.q` file
**Error log:** `BuildResult.txt` in the same folder

---

### `ClearBuildOutput`

**Purpose:** Clears the error/output text area in the UI before a new build run.

---

### `SetUIState(bool running)`

**Purpose:** Enables or disables UI buttons during the build process to prevent re-entry.
- `running = true` → disables Browse/Execute buttons, shows progress indicator
- `running = false` → re-enables buttons

---

## Script Reading Functions

---

### `ReadLanguageSection`

**Signature:**
```csharp
private void ReadLanguageSection(TextReader txtReader, List<string> lines,
    ref int a, ref int b, Dictionary<int,int> dicLine)
```

**Purpose:** Reads one `@LANGUAGE` section from the open `TextReader` into `lines`.
Continues reading until the next `@LANGUAGE` marker or end-of-file.

**Input:**
- `txtReader` — open stream positioned after a `@LANGUAGE` line
- `lines` — the list to populate with cleaned lines for this language
- `a`, `b` — running counters (logical line number, physical line number)
- `dicLine` — maps logical index to physical file line number

**Processing:**
- Skips blank lines, comment lines starting with `#` or `$`
- Trims whitespace and collapses multiple spaces via `Regex.Replace(s, @"\s+", " ")`
- Stops at next `@LANGUAGE` or EOF

---

## List & Grid List Parsers

---

### `prepareList`

**Signature:**
```csharp
private int prepareList(List<string> lines, int i,
    TextWriter txtWriter, Dictionary<int,int> dicLine)
```

**Purpose:** Parses a `*LIST "name"` block and stores it in `dicListNameVsList`.

**Input:**
- `lines` — full script line array
- `i` — index of the `*LIST` line
- `txtWriter` — error log writer
- `dicLine` — physical line number map

**Processing:**
1. Extracts the list name from `*LIST "name"`
2. Reads subsequent lines until another `*` directive or end of block
3. Each attribute line must be in `value:label` format
4. Validates: numeric value, unique values, unique labels (case-insensitive)
5. Creates `AttributeMain` objects and stores in `dicListNameVsList[listName]`

**Returns:** Updated index `i` pointing to the last consumed line

**Output:** Populates `dicListNameVsList` dictionary

---

### `prepareListForLanguage`

**Signature:**
```csharp
private int prepareListForLanguage(List<string> linesLanguage, int i,
    TextWriter txtWriter, Dictionary<int,int> dicLine, int ln, int langNo)
```

**Purpose:** Same as `prepareList` but for a language-specific section (Language 1–9).

**Input:**
- `linesLanguage` — lines for the specific language section
- `ln` — line offset for this language section (used for error reporting)
- `langNo` — language number (1–9)

**Output:** Populates `dicQidVsAttributeListLan1` through `dicQidVsAttributeListLan9`
based on `langNo`

---

### `prepareGridList`

**Signature:**
```csharp
private int prepareGridList(List<string> lines, int i,
    List<string> listOfGridListForDupliCheck,
    TextWriter txtWriter, Dictionary<int,int> dicLine)
```

**Purpose:** Parses a `*GRIDLIST "name"` block and stores it in `dicGridListNameVsList`.

**Input:**
- `lines`, `i`, `txtWriter`, `dicLine` — same as `prepareList`
- `listOfGridListForDupliCheck` — list used to detect duplicate grid list names

**Processing:**
1. Extracts grid list name from `*GRIDLIST "name"`
2. Checks for duplicate grid list names
3. Reads attribute lines in `value:label` format
4. Stores in `dicGridListNameVsList[name]` as `List<GridInfo>`

**Returns:** Updated index `i`

---

### `prepareGridListForLanguage`

**Signature:**
```csharp
private int prepareGridListForLanguage(List<string> linesLanguage, int i,
    TextWriter txtWriter, Dictionary<int,int> dicLine, int ln, int langNo)
```

**Purpose:** Same as `prepareGridList` for a language-specific section.

**Output:** Populates `dicGridListNameVsListLan1` through `dicGridListNameVsListLan9`

---

## Logic Parsers

---

### `prepareIf`

**Signature:**
```csharp
private int prepareIf(List<string> lines, int i,
    List<string> listOfQuestionIdForDupliCheck,
    List<AutoResponse> listOfAutoResponseTemp,
    List<LogicalSyntax> listOfLogicalSyntaxTemp,
    TextWriter txtWriter, Dictionary<int,int> dicLine)
```

**Purpose:** Parses a standalone `*IF [condition] *ACTION` block between questions.

**Input:**
- `lines`, `i` — current position in the script
- `listOfQuestionIdForDupliCheck` — all known QIds (for validation)
- `listOfAutoResponseTemp` — accumulates `*INCLUDE`/`*EXCLUDE` rules
- `listOfLogicalSyntaxTemp` — accumulates `*GOTO`/`*MSG` rules
- `txtWriter`, `dicLine` — error reporting

**Processing:**
1. Splits the line on `*` — expects exactly 3 parts
2. Extracts condition from `[...]` brackets
3. Validates condition via `checkLogicalExp.checkIfCondition()`
4. Special path for `REGULAREXPOF` conditions (allows `*` in the pattern)
5. Dispatches to action handler:
   - `GOTO` → creates `LogicalSyntax` with `LogicTypeId = "3"`
   - `MSG` → creates `LogicalSyntax` with `LogicTypeId = "2"`
   - `INCLUDE`/`EXCLUDE` → creates `AutoResponse` with `LogicId = "1"`
6. Validates all referenced QIds exist

**Returns:** Index `i` (unchanged — single-line directive)

---

### `prepareIncludeExclude`

**Signature:**
```csharp
private int prepareIncludeExclude(List<string> lines, int i,
    List<string> listOfQuestionIdForDupliCheck,
    List<AutoResponse> listOfAutoResponseTemp,
    TextWriter txtWriter, Dictionary<int,int> dicLine)
```

**Purpose:** Parses a **standalone** (unconditional) `*INCLUDE` or `*EXCLUDE` line.

**Processing:**
Identical to the INCLUDE/EXCLUDE branch inside `prepareIf`, but without a condition.
Creates `AutoResponse` with empty `IfCondition`.

Supports all the same value formats:
- `[val1;val2;val3]` — numeric list
- `[low TO high]` — range
- `SourceQId` or `SourceQId.N` — reference
- All logic functions (`ASCRANKOf`, `SUMOf`, etc.)

**Returns:** Index `i` (unchanged)

---

## Main Question Parser

---

### `prepareQuestion`

**Signature:**
```csharp
private int prepareQuestion(
    List<string> lines, int i,
    List<string> listOfQuestionIdForDupliCheck,
    List<string> listOfGridListForDupliCheck,
    List<LogicalSyntax> listOfLogicalSyntaxTemp,
    List<Question> listOfQuestionTemp,
    Question currentQuestionTemp,
    Dictionary<string, List<AttributeMain>> dicQidVsAttributeListTemp,
    List<AttributeFilter> listOfAttributeFilterTemp,
    TextWriter txtWriter,
    Dictionary<int,int> dicLine,
    AttributeMain attributeMainR = null)   // optional, used by *REPEAT
```

**Purpose:** The **core question parser**. Processes a single `*QUESTION` block including
its header line, question text, and all attribute lines.

**Input:**
- `lines`, `i` — current position at the `*QUESTION` line
- Various accumulator lists/dictionaries for this question's output
- `attributeMainR` — optional override attribute when called from a `*REPEAT` expansion

**Processing — Phase 1: Header line parsing**

Splits the `*QUESTION` header on `*` and processes each token:

| Token | Field Set |
|-------|-----------|
| `QUESTION <QId>` | `myQuestion.QId`, validates format, checks duplicates |
| `*SR`, `*MR`, `*OPEN`, etc. | `myQuestion.QType` |
| `*RANDOM` | `HasRandomAttrib = "2"` |
| `*ROT` | `HasRandomAttrib = "1"` |
| `*FROT` | `HasRandomAttrib = "5"` |
| `*OTPGROUPROT` | `HasRandomAttrib = "10"` |
| `*OTPROTGROUP` | `HasRandomAttrib = "01"` |
| `*OTPROTGROUPROT` | `HasRandomAttrib = "11"` |
| `*QROT` | `HasRandomQntr = "1"` |
| `*GROUPROT n` | `HasMessageLogic = n` |
| `*FONTSIZE n` | `WrittenOEInPaper = n` |
| `*MIN n` | `NoOfResponseMin = n` |
| `*MAX n` | `NoOfResponseMax = n` |
| `*COLUMN n` | `NumberOfColumn = n` |
| `*HORIZONTAL` | `NumberOfColumn = "2"` |
| `*TAKEONLYONE` | `NumberOfColumn = "4"` |
| `*SHOWASFORM` | `NumberOfColumn = "3"` |
| `*ADDSEARCH` | `NumberOfColumn = "99"` |
| `*DUMMY1` | `HasAutoResponse = "1"` |
| `*DUMMY2` | `HasAutoResponse = "2"` |
| `*DELAY n` | `ShowInReport = n` |
| `*NOBACKBTN` | `DisplayBackButton = "1"` |
| `*NONEXTBTN` | `DisplayNextButton = "1"` |
| `*GROT` | `ForceToTakeOE = "1"` |
| `*GRANDOM` | `ForceToTakeOE = "2"` |
| `*INRLD` | `ForceToTakeOE = "1"` |
| `*ADDRESS1-4` | `DisplayJumpButton = "1"/"2"/"3"/"4"` |
| `*TBC` | `DisplayJumpButton = "TBC"` |
| `*IMGADJBY n` | `HasMediaPath = n` |
| `*JUMPFOR n` | `ResumeQntrJump = n` |
| `*BLOCK n` | `ResumeQntrJump = n` |
| `*DIRIMAGE` | `WrittenOEInPaper = "1"` |
| `*SHOWASNUMTEXT` | `WrittenOEInPaper = "1"` |
| `*PICT "path"` | `myQuestion.FilePath = path` |
| `*VIDEO "path"` | `myQuestion.FilePath = path` |
| `*QLABEL "text"` | `myQuestion.Comments = text` |
| `*INCLUDE [QId]` | `myAttributeFilter.FilterType = "1"` → T_OptAttrbFilter |
| `*INCLUDEBYORDER [QId]` | `myAttributeFilter.FilterType = "5"` |
| `*EXCLUDE [QId]` | `myAttributeFilter.FilterType = "2"` |
| `*INCLUDEGRIDLIST [QId]` | Sets `GridFilterQId`, `GridFilterType = "1"` |
| `*USEGRIDLIST "name"` | Sets `currentGridListName` |
| `*DKCS "label" "code"` | Auto-creates 2 DKCS attributes |
| `*FIFS` | Auto-creates 4 FIFS attributes (FI Name/Code, FS Name/Code) |
| `*IF [condition]` | Stores `LogicalSyntax` with `LogicTypeId = "4"` |

**Processing — Phase 2: Question text**

Reads lines after the header until an attribute line (starts with digit or `*`) is found.
- Concatenates lines with `<br>` separator
- Validates `{QId}` and `{QId.N}` curly references via `CheckCurlyReferences()`
- Error if question text is empty

**Processing — Phase 3: Attribute lines**

Reads each attribute line until a new `*` directive:

Attribute line formats:
```
value:label
value:label *OPEN
value:label *NMUL
value:label *NOCON
value:label *MANDATORY "message"
value:label *MIN n *MAX n
value:label *PICT "path"
value:label *GROUPNAME "name"
value:label *GROUPHEAD
value:label *LAT
value:label *LON
value:label *COMPVAL n
value:label *SR/*MR/*NUMBER/*DROPDOWN/*AUTOCOMPLETE/*DATE/*TIME    (for *FORM)
value:label *USEGRIDLIST "name"   (for grid row with its own grid)
value:label *EXCEPT value2
```

Handles `*USELIST "name"` line to expand a pre-defined list.

For grid question types (`*GRIDSR`, `*GRIDMR`, `*PSCALE`, `*DRAGDROP`):
- Sets `LinkId1 = "1"` and `LinkId2 = currentGridListName` on each attribute

**Processing — Phase 4: Post-attribute finalization**

- Inserts DKCS, FIFS, or SingleDropdown auto-attributes at the start of attribute list
- Adds question to `listOfQuestionTemp`
- Adds attributes to `dicQidVsAttributeListTemp[QId]`
- Adds attribute filter to `listOfAttributeFilterTemp`

**Returns:** Updated index `i` pointing to the last consumed line

---

### `prepareQuestionForLanguage`

**Signature:**
```csharp
private int prepareQuestionForLanguage(List<string> linesLanguage, int i,
    TextWriter txtWriter, Dictionary<int,int> dicLine, int ln, int langNo)
```

**Purpose:** Parses a `*QUESTION` block within a `@LANGUAGE` section.
Only captures question text and attribute text (no type/modifier parsing).
Updates the corresponding language fields (`QuestionLang3–10`, `AttributeLang3–10`).

**Input:**
- `linesLanguage` — lines for the specific language
- `ln` — line offset
- `langNo` — 1–9, maps to Lang3–Lang10 fields

**Matching strategy:** Matches language questions to English questions by QId.
Updates `dicQidVsAttributeListLan{N}` with translated attribute labels.

---

## Repeat Block Functions

---

### `BuildRepeatIterationList`

**Signature:**
```csharp
private List<string> BuildRepeatIterationList(string repeatSource,
    TextWriter txtWriter, int lineNum)
```

**Purpose:** Builds the list of iteration values for a `*REPEAT [source]` block.

**Input:**
- `repeatSource` — the content of `[...]` from the `*REPEAT` line
  - Can be a list name (string matching a key in `dicListNameVsList`)
  - Can be a QId (matching a key in `dicQidVsAttributeList`)

**Output:** `List<string>` of attribute values/labels to iterate over

---

### `ExpandRepeatBlockEnglish`

**Signature:**
```csharp
private void ExpandRepeatBlockEnglish(
    List<string> repeatBuffer,
    List<int> repeatLineNums,
    List<string> iterationList,
    List<string> listOfQuestionIdForDupliCheck,
    List<string> listOfGridListForDupliCheck,
    TextWriter txtWriter)
```

**Purpose:** Expands a `*REPEAT` block in the English section by iterating over `iterationList`
and calling `prepareQuestion` / `prepareList` / `prepareGridList` for each iteration.

Each iteration substitutes the placeholder token with the current iteration value.

---

### `ExpandRepeatBlockLanguage`

**Signature:**
```csharp
private void ExpandRepeatBlockLanguage(
    List<string> repeatBuffer,
    List<int> repeatLineNums,
    List<string> iterationList,
    int langNo,
    TextWriter txtWriter)
```

**Purpose:** Same as `ExpandRepeatBlockEnglish` but for a language-specific section.

---

## Validation Helpers

---

### `CheckCurlyReferences`

**Signature:**
```csharp
private void CheckCurlyReferences(string line, int lineNum, TextWriter txtWriter)
```

**Purpose:** Validates all `{QId}` and `{QId.N}` references in a question text line.

**Input:**
- `line` — one line of question text
- `lineNum` — physical file line number for error reporting

**Processing:**
- Uses compiled regex `_curlyRefRegex` = `\{([^}]+)\}` to find all `{...}` tokens
- Each match is checked against `_globalQIds` (all QIds registered so far)
- If the referenced QId doesn't exist yet, logs a warning to `txtWriter`

**Note:** `_globalQIds` is populated when each `*QUESTION` QId is first registered (line 2649),
so forward-references are flagged.

---

### `isAttribute`

**Signature:**
```csharp
private bool isAttribute(string line)
```

**Purpose:** Determines whether a line is an attribute definition (as opposed to question text).

**Logic:**
- Returns `true` if the line starts with a digit (e.g., `1:Male`)
- Returns `true` if the line starts with `*USELIST` or `*USEGRIDLIST`
- Returns `false` for question text lines

---

### `replaceNull`

**Signature:**
```csharp
private string replaceNull(string value)
```

**Purpose:** Sanitises a value before insertion into SQL.
- Returns `""` if `value` is `null`
- Escapes single quotes by doubling them (`'` → `''`) to prevent SQL injection

---

## Database Functions

---

### `getShellDB`

**Signature:**
```csharp
private bool getShellDB()
```

**Purpose:** Copies the shell (template) SQLite database to the output location before inserting data.

**Processing:**
1. Looks for the shell DB file at `C:\Temp\ShellDB\<shell_filename>.db`
2. Copies it to `<script_directory>\<DatabaseName>.db`
3. Returns `true` on success, `false` on failure

**Why shell DB:** The shell database contains the empty table schema with all required columns.
Rather than creating tables from scratch, the compiler copies the schema and then populates it.

---

## Key Data Structures

### `Question` (model class)

| Field | Type | Description |
|-------|------|-------------|
| `QId` | string | Question identifier |
| `QuestionEnglish` | string | English question text (with `<br>` separators) |
| `QType` | string | Numeric type ID (1–60) |
| `NoOfResponseMin` | string | Minimum responses (`*MIN`) |
| `NoOfResponseMax` | string | Maximum responses (`*MAX`) |
| `HasAutoResponse` | string | Auto-fill mode (`1`=DUMMY1, `2`=DUMMY2) |
| `HasRandomAttrib` | string | Rotation type (see table below) |
| `HasRandomQntr` | string | Question rotation (`1`=yes) |
| `NumberOfColumn` | string | Layout columns (1-4, 99=search) |
| `ShowInReport` | string | Delay milliseconds |
| `HasMessageLogic` | string | Group rotation count |
| `WrittenOEInPaper` | string | Font size or display flag |
| `ForceToTakeOE` | string | Group rotate/random/reload flag |
| `HasMediaPath` | string | Image adjustment value |
| `DisplayBackButton` | string | Back button flag |
| `DisplayNextButton` | string | Next button flag |
| `DisplayJumpButton` | string | Address/TBC jump type |
| `ResumeQntrJump` | string | Jump/block count |
| `FilePath` | string | Media file path |
| `Comments` | string | Question label/comment |
| `SilentRecording` | string | Recording field name |
| `OrderTag` through `OrderTag5` | string | Display order indices |
| `QuestionLang3` through `QuestionLang10` | string | Language translations |

**HasRandomAttrib values:**

| Value | Keyword | Meaning |
|-------|---------|---------|
| `"1"` | `*ROT` | Rotate options (fixed order rotation) |
| `"2"` | `*RANDOM` | Randomise options |
| `"5"` | `*FROT` | Fixed rotation |
| `"10"` | `*OTPGROUPROT` | Rotate options, NOT groups |
| `"01"` | `*OTPROTGROUP` | Rotate within group, NOT options |
| `"11"` | `*OTPROTGROUPROT` | Rotate both options and groups |

---

### `AttributeMain` (model class)

| Field | Type | Description |
|-------|------|-------------|
| `QId` | string | Parent question |
| `AttributeEnglish` | string | Option label text |
| `AttributeValue` | string | Numeric code |
| `AttributeOrder` | string | Display sequence |
| `TakeOpenended` | string | Allow OE text (`"1"`) |
| `IsExclusive` | string | Mutually exclusive (`"1"`=NMUL, `"2"`=NOCON) |
| `LinkId1` | string | Sub-type reference (see table below) |
| `LinkId2` | string | Grid list name |
| `MinValue` | string | Min value / DKCS flag |
| `MaxValue` | string | Max value |
| `ForceAndMsgOpt` | string | Mandatory marker (`"11"`) or message |
| `GroupName` | string | Group label |
| `FilterQid` | string | Filter source question |
| `FilterType` | string | Filter type |
| `ExcepValue` | string | Exception value |
| `Comments` | string | Additional info (image path, grouphead) |
| `AttributeLang3–10` | string | Language translations |

**LinkId1 values:**

| Value | Meaning | Used For |
|-------|---------|---------|
| `"1"` | Single Response | Grid rows, SingleDropdown |
| `"2"` | Multiple Response | Grid MR rows |
| `"3"` | Alpha / Text | ALPHALIST, FIFS fields |
| `"4"` | Number | NUMLIST, NUMLISTTOTAL |
| `"14"` | Date | DATE in grid/form |
| `"15"` | Time | TIME in grid/form |
| `"22"` | Autocomplete | AUTOCOMPLETE in form |
| `"24"` | Dropdown | DROPDOWN in form |
| `"27"` | Grid Numeric | GRIDNUM |

---

### `LogicalSyntax` (model class)

| Field | Type | Description |
|-------|------|-------------|
| `QId` | string | Question the logic is attached to |
| `ThenValue` | string | Target QId (GOTO), message (MSG), or own QId (IF) |
| `LogicTypeId` | string | `"2"`=MSG, `"3"`=GOTO, `"4"`=Question IF |
| `IfCondition` | string | Raw condition expression |

---

### `AutoResponse` (model class)

| Field | Type | Description |
|-------|------|-------------|
| `QId` | string | Target question to filter |
| `ThenValue` | string | `"Include[...]"` or `"Exclude[...]"` |
| `LogicId` | string | Always `"1"` |
| `IfCondition` | string | Condition (empty for unconditional) |

---

### `AttributeFilter` (model class)

| Field | Type | Description |
|-------|------|-------------|
| `ProjectId` | string | From PROJECT CODE |
| `QId` | string | Question being filtered |
| `InheritedQId` | string | Source question |
| `FilterType` | string | `"1"`=INCLUDE, `"2"`=EXCLUDE, `"5"`=INCLUDEBYORDER |

---

## Error Reporting

All validation errors are written to `BuildResult.txt` in the same folder as the script.

**Format:**
```
Line : <physical_line_number> <error_description>
```

**Common error messages:**

| Message | Cause |
|---------|-------|
| `Duplicate QId <id>` | Same QId used more than once |
| `Invalid QId <id>, Must be started with Alpha` | QId fails regex `^[a-zA-Z0-9]+$` |
| `<id> should not be used as QId` | QId is a reserved SQL keyword |
| `Question Type must be exist` | No type keyword found on `*QUESTION` line |
| `Duplicate Token` | Same modifier used twice on one question |
| `Invalid Question Text : should not exist` | Empty question text |
| `Invlaid Syntax/Incorrect Qid <condition>` | Condition fails `checkIfCondition()` |
| `Invlaid IF Statement` | `*IF` line doesn't split into exactly 3 `*` parts |
| `*REPEAT block not closed with *ENDREPEAT` | Missing `*ENDREPEAT` |
| `RespName question is missing` | Mandatory QId absent |
| `RespMobile question is missing` | Mandatory QId absent |
| `Centre question is missing` | Mandatory QId absent |
| `FIFSInfo question is missing` | Mandatory QId absent |

**Build success** is indicated by the absence of error lines in `BuildResult.txt`
and the successful creation of the `.db` output file.
