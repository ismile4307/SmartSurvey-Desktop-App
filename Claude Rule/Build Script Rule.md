# SmartSurvey Build Script Rules

Extracted from `DBI Scripting/Forms/Scripting/FrmBuildScript.xaml.cs`

---

## 1. Script File Structure

A `.q` script file is read line-by-line with the following rules applied:

- **Blank lines** → skipped entirely
- **Lines starting with `#`** → comment, skipped
- **Lines starting with `$`** → skipped
- **All other lines** → trimmed, consecutive whitespace collapsed to single space, then processed

The file is split into sections by `@LANGUAGE` markers:
- Everything before the first `@LANGUAGE` = **English (main)** section
- Each subsequent `@LANGUAGE` marker starts a new language section (up to 9 languages)

---

## 2. Header / Script Info Rules

The first lines of the English section must define these fields using `FIELD: value` format:

| Field | Required |
|-------|----------|
| `PROJECT NAME: value` | Yes |
| `PROJECT CODE: value` | Yes |
| `SCRIPT VERSION: value` | Yes |
| `SCRIPT NAME: value` | Yes |
| `SCRIPTED BY: value` | Yes |

**Errors if missing:**
```
Line : 3 Project Name Missing
Line : 4 Project Code Missing
Line : 5 Script Version Missing
Line : 6 Script Name Missing
Line : 8 Scripted by name Missing
```

---

## 3. @LANGUAGE Declaration

```
@LANGUAGE "LanguageName"
```

- Must split by `"` into exactly **3 parts** → `['@LANGUAGE ', 'LanguageName', '']`
- Invalid syntax → **"Invalid @LANGUAGE Syntax"** shown in MessageBox
- Up to **9 language sections** supported (Language 1–9)

Language mapping in database:
- Language 1 → Bengali (DisplayOrder=2, `QuestionBengali` column)
- Language 2 → 3rd translation (DisplayOrder=3, `QuestionLang3`)
- Languages 3–9 → DisplayOrder 4–10, `QuestionLang4`–`QuestionLang10`

---

## 4. *LIST Rules

### Syntax
```
*LIST "ListName"
1:Label
2:Label *PROPERTY *PROPERTY
3:Label
```

### Validation
- **List name**: Extracted from quotes; must be unique (no duplicate list names)
- **Attribute line format**: `numeric:label` — colon-separated, exactly 2 parts
- **Attribute value (left of colon)**: Must match `^\d+$` (numeric only)
- **Attribute value uniqueness**: No duplicate values within the same list
- **Attribute label uniqueness**: Case-insensitive, no duplicates within the same list

### Attribute Properties (English section only)

| Property | Effect |
|----------|--------|
| `*OPEN` | TakeOpenended = "1" |
| `*ALPHA` | LinkId1 = "3" |
| `*NUMBER` | LinkId1 = "4" |
| `*NMUL` | IsExclusive = "1" |
| `*MANDATORY` | ForceAndMsgOpt = "11" |
| `*PICT "file"` | FilePath = "file" (must split by `"` into exactly 3 parts) |
| `*VIDEO "file"` | FilePath = "file" (must split by `"` into exactly 3 parts) |
| `*MIN n` | MinValue = n (must match `^\d+$` or `^\d.+$`) |
| `*MAX n` | MaxValue = n (must match `^\d+$` or `^\d.+$`) |

> **Note:** Language sections (`*LIST` inside `@LANGUAGE` blocks) do **not** support attribute properties — only plain `numeric:label` pairs are allowed.

---

## 5. *GRIDLIST Rules

### Syntax
```
*GRIDLIST "GridListName"
1:Label
2:Label *PROPERTY
3:Label
```

### Validation
- **Grid list name**: Unique; in quotes
- **Attribute format**: Same as `*LIST` — `numeric:label`
- **Numeric values**: Must be `^\d+$`
- **Duplicate checks**: Values and labels (case-insensitive) must be unique per grid list

### Attribute Properties

| Property | Effect |
|----------|--------|
| `*OPEN` | TakeOpenended = "1" |
| `*NMUL` | IsExclusive = "1" |
| `*MANDATORY` | ForceAndMsgOpt = "11" |
| `*PICT "file"` | FilePath via quoted syntax |
| `*VIDEO "file"` | FilePath via quoted syntax |
| `*MIN n` | MinValue |
| `*MAX n` | MaxValue |

> Language-section `*GRIDLIST` blocks do **not** allow attribute properties.

---

## 6. *QUESTION Rules

### General Syntax
```
*QUESTION QId *TYPE *PROPERTY *PROPERTY
Question text (one or more lines)
1:Attribute1 *PROPERTY
2:Attribute2
*USELIST "ListName"
```

### Question ID (QId) Rules
- Must match `^[a-zA-Z0-9]+$` (alphanumeric, no spaces/symbols)
- Must be unique across the entire script
- **Banned QId words** (cannot be used as a QId):

  | UNION | ABS | JOIN | SELECT | INTO |
  |-------|-----|------|--------|------|
  | WHERE | IF | EXISTS | ORDER | BY |
  | UPDATE | DELETE | MAX | MIN | |

### Question Text Rules
- Must be present and non-empty
- Appears on the line(s) immediately after the `*QUESTION` declaration line
- Leading/trailing whitespace trimmed

### Mandatory Questions
These QIds **must** exist in every script:

| QId | Description |
|-----|-------------|
| `RespName` | Respondent name |
| `RespMobile` | Respondent mobile number |
| `Centre` | Centre/location |
| `FIFSInfo` | FI/FS information |

Every script must also have:
- At least **one** `*END` question
- At least **one** `*TERMINATE` question

Errors if missing:
```
RespName question is missing..
RespMobile question is missing..
Centre question is missing..
FIFSInfo question is missing..
End Question not exist
Terminate Question not exist
```

---

## 7. Question Types

| Token | QType | Description |
|-------|-------|-------------|
| `*SR` | 1 | Single Response |
| `*MR` | 2 | Multi Response |
| `*OPEN` | 3 | Open Ended (alpha) |
| `*NUMBER` | 4 | Numeric |
| `*RANK` | 5 | Rank |
| `*IMAGE` | 6 | Image display |
| `*GRIDSR` | 7 | Grid Single Response |
| `*GRIDMR` | 8 | Grid Multi Response |
| `*MEDIA` | 9 | Media |
| `*RECORDING` | 10 | Audio recording |
| `*ALPHALIST` | 12 | Alphabetic list |
| `*NUMLIST` | 13 | Numeric list |
| `*DATE` | 14 | Date |
| `*TIME` | 15 | Time |
| `*CAPTUREIMAGE` | 16 | Camera capture |
| `*NUMLISTTOTAL` | 17 | Numeric list with total |
| `*AUTOCOMPLETE` | 22 | Autocomplete |
| `*AUTOCOMPLETEANS` | 23 | Autocomplete answer |
| `*DROPDOWN` | 24 | Dropdown |
| `*DRAGDROP` | 26 | Drag and drop |
| `*GRIDNUM` | 27 | Grid numeric |
| `*SCALE7` | 32 | 7-point scale |
| `*MAXDIFF` | 40 | MaxDiff |
| `*GPS` | 41 | GPS location |
| `*FORM` | 48 | Form (multi-field) |
| `*INFO` | 49 | Info/display only |
| `*END` | 50 | End of interview |
| `*TERMINATE` | 51 | Terminate interview |
| `*FIFS` | 60 | FI/FS question |
| `*SCALE10` | 61 | 10-point scale |

---

## 8. Question Properties

| Property | Effect |
|----------|--------|
| `*RANDOM` | HasRandomAttrib = "2" |
| `*ROT` | HasRandomAttrib = "1" (rotate options) |
| `*QROT` | HasRandomQntr = "1" |
| `*GROUPROT n` | HasMessageLogic = n (n must be numeric) |
| `*FONTSIZE n` | WrittenOEInPaper = n (numeric) |
| `*MIN n` | NoOfResponseMin = n (numeric) |
| `*MAX n` | NoOfResponseMax = n (numeric) |
| `*COLUMN n` | NumberOfColumn = n (numeric) |
| `*HORIZONTAL` | NumberOfColumn = "2" |
| `*IMGADJBY n` | HasMediaPath = n (numeric) |
| `*IMGSIZE n` | HasMediaPath = n (numeric) |
| `*JUMPFOR n` | ResumeQntrJump = n (numeric) |
| `*BLOCK n` | ResumeQntrJump = n (numeric) |
| `*DUMMY1` | HasAutoResponse = "1" |
| `*DUMMY2` | HasAutoResponse = "2" |
| `*DELAY n` | ShowInReport = n (numeric) |
| `*NOBACKBTN` | DisplayBackButton = "1" |
| `*NONEXTBTN` | DisplayNextButton = "1" |
| `*EXTCAMERA` | ForceToTakeOE = "1" |
| `*INRLD` | ForceToTakeOE = "1" |
| `*ADDRESS1`–`*ADDRESS4` | DisplayJumpButton = "1"–"4" |
| `*SHOWASFORM` | NumberOfColumn = "3" |
| `*DIRIMAGE` | WrittenOEInPaper = "1" |
| `*SHOWASNUMTEXT` | WrittenOEInPaper = "1" |
| `*PICT "file"` | FilePath = "file" |
| `*VIDEO "file"` | FilePath = "file" |

---

## 9. *DKCS Property Rules

Adds a "Don't Know / Can't Say" option to a question.

### Syntax
```
*DKCS "Label" "Code"
```

- Split by `"` must yield exactly **5 parts**: `['*DKCS ', 'Label', ' ', 'Code', '']`
- Label must **not** be empty
- Code must be numeric: `^\d+$`

Creates 2 predefined attributes:
1. `AttributeValue="1"`, `MinValue="5"` (the "none" marker)
2. `AttributeValue=Code`, `AttributeEnglish=Label`, `IsExclusive="1"`

---

## 10. *FIFS Property Rules

Attached to a question of QType 60.

Automatically creates **4 mandatory attributes**:

| # | Label | LinkId1 | ForceAndMsgOpt |
|---|-------|---------|---------------|
| 1 | FI Name | "3" (Alpha) | "11" (Mandatory) |
| 2 | FI Code | "3" | "11" |
| 3 | FS Name | "3" | "11" |
| 4 | FS Code | "3" | "11" |

---

## 11. *USELIST Rules

Attach an existing list to a question instead of defining attributes inline.

### Syntax
```
*USELIST "ListName"
```

- List name must exist in `dicListNameVsList` (previously defined `*LIST`)
- Error if not found: `"Invlaid use list name"`
- No additional attribute lines allowed after `*USELIST`

---

## 12. *USEGRIDLIST Rules

Attach a grid list to an attribute.

### Syntax
```
1:Label *USEGRIDLIST "GridListName"
```

- Split by space → exactly **2 parts**
- Grid list name in quotes: must match `^[a-zA-Z0-9]+$`
- Grid list must exist in `dicGridListForDupliCheck`
- Sets attribute `LinkId2 = GridListName`
- For **Form questions** (`*FORM`): attributes using `*SR`, `*MR`, `*DROPDOWN`, or `*AUTOCOMPLETE` **must** have `*USEGRIDLIST`
  - Error: `"USEGRIDLIST must be exist for Form attribute"`

---

## 13. *IF / *INCLUDE / *EXCLUDE Rules

### *IF (Conditional Logic)

```
*QUESTION QId *SR *IF [condition]
```

- Condition is extracted between `[` and `]`
- Validated by `CheckLogicalExp` class
- **MOBILENUMBER special case**: If condition contains `MOBILENUMBER`, the `[` character must appear exactly once
  - Error: `"Incorrect expression [ must be exist at least one"`

### *INCLUDE / *EXCLUDE

```
*QUESTION QId *MR *INCLUDE [PreviousQId]
*QUESTION QId *MR *EXCLUDE [PreviousQId]
```

- `[PreviousQId]` must match `^[a-zA-Z0-9]+$`
- Referenced QId must already exist in the script (already-processed questions)
- Error if not found: `"Invalid QId : X"`

---

## 14. *REPEAT / *ENDREPEAT Block Rules

### Syntax
```
*REPEAT [source]
  *QUESTION ?R_QuestionSuffix *SR
  Question text ?R
  1:Option1
  2:Option2
*ENDREPEAT
```

### Source Types

**1. Numeric range:**
```
*REPEAT [1 TO 5]
```
- Format: `^\d+ TO \d+$` (case-insensitive)
- Start must be **less than** end (error if start >= end)
- Generates integers from start to end (inclusive)

**2. Question reference:**
```
*REPEAT [PreviousQId]
```
- QId must exist in `dicQidVsAttributeList`
- Iterates over each attribute value; stops at an attribute whose label contains "None"

### Variable Substitution
- `?R` is the placeholder — replaced with the current iteration value
- All generated QIds are pre-registered before parsing to allow cross-iteration `*IF` references

### Validation
- Every `*REPEAT` must be closed with `*ENDREPEAT`
- Error if not closed: `"*REPEAT block not closed with *ENDREPEAT"`
- Error for invalid source syntax: `"*REPEAT syntax invalid — missing [source]"`

---

## 15. *STARTREC / *ENDREC (Recording) Rules

### Syntax
```
*STARTREC "RecordingName"
  ... questions inside recording ...
*ENDREC
```

### Validation
- `*STARTREC`: Split by `"` must yield exactly **3 parts** → name is `parts[1]`
  - Error: `"Invlaid Syntax"` if not 3 parts
- `*ENDREC`: Trimmed string must be exactly **7 characters** (i.e. `*ENDREC` and nothing else)
  - Error: `"Invlaid Syntax"` if extra content follows

---

## 16. {QId} / {QId.N} Inline Reference Validation

Question text may reference another question's answer using curly braces.

### Supported Formats
- `{QId}` — reference the full answer of QId
- `{QId.N}` — reference attribute N of QId

### Validation Rules
- Pattern: `\{([^}]+)\}` (all `{...}` occurrences in question text)
- The QId part (everything before the first `.` if present) must be in `_globalQIds` (all registered question IDs up to that point in the script)
- **Exact match** → OK
- **Case-insensitive match** → WARNING: `"case mismatch, defined as '{matched}'"`
- **No match** → ERROR: `"QId 'X' is not defined"`

---

## 17. Form Question (*FORM) Rules

- Every attribute in a `*FORM` question must have a **type** property:
  - `*SR`, `*MR`, `*ALPHA`, `*NUMBER`, `*DROPDOWN`, `*AUTOCOMPLETE`
- Attributes using `*SR`, `*MR`, `*DROPDOWN`, or `*AUTOCOMPLETE` **must also** have `*USEGRIDLIST`
  - Error: `"USEGRIDLIST must be exist for Form attribute"`
- A `*FORM` question with zero attributes → Error: `"QId - Form Question has no attribute"`

---

## 18. Post-Build (checkScript) Validation

Runs after all questions are parsed.

| Check | Error |
|-------|-------|
| `*FORM` question has no attributes | `"QId - Form Question has no attribute"` |
| `*FIFS` question: attributes missing `LinkId1` | `"Invalid attribute properties, Form Question must have qtype"` |
| `*FIFS` question: attributes with LinkId1 1 or 2 missing `LinkId2` | Same error |
| `*SR` or `*MR` question: attribute has `LinkId1` or `LinkId2` | `"Attribute properties should not exist"` |

---

## 19. Language Cross-Check Rules

Called via `checkEnglishBengaliScript()` for each language section.

For **every English question** (excluding `DUMMY1`/`DUMMY2` auto-responses):
- The same QId must exist in the language section
  - Error: `"QId not exit in Language X"`
- Number of attributes must **exactly match** the English count
  - Error: `"QId Number of attributes are not same X"`
- Each attribute value must match
  - Error: `"QId : Attribute not matched in Language X"`

For **grid lists**:
- Same count as English required
- Same attribute values required

---

## 20. All Valid Script Keywords

**Question type tokens:**
`QUESTION, SR, MR, ALPHA, NUMBER, RANK, IMAGE, GRIDSR, GRIDMR, GRIDNUM, MEDIA, ALPHALIST, NUMLIST, DATE, TIME, CAPTUREIMAGE, NUMLISTTOTAL, AUTOCOMPLETE, AUTOCOMPLETELIST, AUTOCOMPLETEANS, DROPDOWN, DROPDOWNLIST, DRAGDROP, FORM, INFO, MAXDIFF, GPS, RECORDING, SCALE7, SCALE10, FIFS, END, TERMINATE`

**Property / constraint tokens:**
`IF, RANDOM, ROT, QROT, FROT, GROUPROT, BLOCK, GRANDOM, GROT, OTPGROUPROT, OTPROTGROUP, OTPROTGROUPROT, MIN, MAX, MANDATORY, COLUMN, INCLUDE, INCLUDEBYORDER, EXCLUDE, FILTER, GOTO, DELAY, EXCEPT, FONTSIZE, IMGADJBY, LAT, LON, COMPVAL`

**List / structure tokens:**
`LIST, USELIST, GRIDLIST, USEGRIDLIST, OPEN, NMUL, NOCON, DKCS, REPEAT, ENDREPEAT, STARTREC, ENDREC, NOBACKBTN, NONEXTBTN, SHOWASFORM, DIRIMAGE, SHOWASNUMTEXT, INRLD, QLABEL, IDOF, EXTCAMERA, ADDRESS1, ADDRESS2, ADDRESS3, ADDRESS4, PICT, VIDEO, TAKEONLYONE, DUMMY1, DUMMY2, HORIZONTAL, JUMPFOR`

---

## 21. Build Success Condition

A build is **successful** only if `BuildResult.txt` is **empty** after all processing. If empty:
- The text `"Build successful..."` is written to the file
- `prepareScriptDB()` is called to generate the SQLite database
- Translation update methods are called for each language present

If any errors were written to `BuildResult.txt`, the database is **not** generated.
