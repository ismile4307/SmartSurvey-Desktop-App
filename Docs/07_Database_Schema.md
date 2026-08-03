# Part 7 — Database Schema Reference

> **Source:** `DBI Scripting\Forms\Scripting\FrmBuildScript.xaml.cs` (INSERT/UPDATE statements)
> **Model classes:** `DBI Scripting\Model\` (Question.cs, AttributeMain.cs, GridInfo.cs, LogicalSyntax.cs, AutoResponse.cs, AttributeFilter.cs)
> **Output:** SQLite `.db` file copied from shell template at `C:\Temp\ShellDB\`

---

## Overview

The compiled `.db` file contains **8 tables**. The compiler writes to them in this order:

| Step | Table | Written by |
|------|-------|-----------|
| 1 | `T_ProjectInfo` | `insertIntoDB()` |
| 2 | `T_Question` | `insertIntoDB()` |
| 3 | `T_OptAttribute` | `insertIntoDB()` |
| 4 | `T_GridInfo` | `insertIntoDB()` |
| 5 | `T_LogicTable` | `insertIntoDB()` |
| 6 | `T_LogicAuto` | `insertIntoDB()` |
| 7 | `T_OptAttrbFilter` | `insertIntoDB()` |
| 8 | `T_LanguageMaster` | `insertIntoDB()` (UPDATE only) |
| 9+ | `T_Question`, `T_OptAttribute`, `T_GridInfo` | `updateBengaliTranslation()`, `update3rdTranslation()` … `update9thTranslation()` |

All string values are stored as TEXT. Numeric-looking values (`AttributeOrder`, `AttributeValue` codes, `LogicId`) may still be stored as quoted strings depending on the context.

**Sentinel rows:** After every question's attributes in `T_OptAttribute` and `T_GridInfo`, the compiler inserts **blank filler rows** (5 blank rows for `T_OptAttribute`, 1 for `T_GridInfo`) as separators. These rows have empty `QId` and `AttributeValue`. Consumers should filter them out with `WHERE QId <> ''`.

Similarly, `T_LogicTable`, `T_LogicAuto`, and `T_OptAttrbFilter` each insert one blank sentinel row after every real row.

---

## 1. `T_ProjectInfo`

One row per database — project metadata.

| Column | Type | Set by | Description |
|--------|------|--------|-------------|
| `ProjectId` | INTEGER | `PROJECT CODE:` header | Numeric project identifier |
| `ProjectName` | TEXT | `PROJECT NAME:` header | Human-readable project name |
| `JobNo` | TEXT | — | Always written as `''` (empty); reserved |
| `Version` | TEXT | `SCRIPT VERSION:` header | Script version string |
| `Status` | TEXT | Compiler | Always `'2'` on initial compile |
| `WebServerAddress` | TEXT | UI setting | Server URL from the build form |

---

## 2. `T_Question`

One row per question in the English body. All columns are TEXT.

| Column | Set by script keyword | Description |
|--------|----------------------|-------------|
| `ProjectId` | `PROJECT CODE:` | Foreign key to `T_ProjectInfo` |
| `QId` | `*QUESTION QId` | Question identifier |
| `QuestionEnglish` | Question text lines | English question wording (trailing `\n\n\n\n` stripped) |
| `QuestionBengali` | `@LANGUAGE` section 1 | Bengali translation (updated post-insert) |
| `AttributeId` | Compiler | Name of the attribute list or grid list used |
| `Comments` | Question text | Same as `QuestionEnglish` unless question has a separate comment field |
| `QType` | Type keyword | See **QType Enumeration** below |
| `NoOfResponseMin` | `*MIN N` | Minimum selections required (`''` if not set) |
| `NoOfResponseMax` | `*MAX N` | Maximum selections allowed (`''` if not set) |
| `HasAutoResponse` | `*INCLUDE`/`*EXCLUDE` | `'1'` if question has auto-response logic, else `''` |
| `HasRandomAttrib` | `*ROT`, `*RANDOM`, `*FROT` | `'1'` if attribute rotation is active, else `''` |
| `NumberOfColumn` | `*COLUMN N` | Number of display columns; `''` if not set |
| `ShowInReport` | Compiler | Reserved; `''` by default |
| `HasRandomQntr` | `*ROT` on question level | `'1'` if question itself participates in rotation |
| `HasMessageLogic` | `*IF [...] *MSG` | `'1'` if question has a message logic rule |
| `WrittenOEInPaper` | Compiler | Reserved; `''` by default |
| `ForceToTakeOE` | Compiler | Reserved; `''` by default |
| `HasMediaPath` | `*PICT`, `*VIDEO` | `'1'` if question has a media file path |
| `DisplayBackButton` | `*NOBACKBTN` | `'0'` if back button hidden; `'1'` otherwise (first question always `'1'`) |
| `DisplayNextButton` | `*NONEXTBTN` | `'0'` if next button hidden; `'1'` otherwise |
| `DisplayJumpButton` | Compiler | Reserved; `''` by default |
| `ResumeQntrJump` | Compiler | Reserved; `''` by default |
| `SilentRecording` | `*RECORDING` | `'1'` if silent audio recording is active |
| `FilePath` | `*PICT "path"`, `*VIDEO "path"` | Media file path string |
| `OrderTag` | Compiler | Sequential insertion index (1-based) |
| `OrderTag1` | Compiler | Same value as `OrderTag` (used for rotation bookkeeping) |
| `OrderTag2` | Compiler | Same value as `OrderTag` |
| `OrderTag3` | Compiler | Same value as `OrderTag` |
| `OrderTag4` | Compiler | Same value as `OrderTag` |
| `OrderTag5` | Compiler | Same value as `OrderTag` |
| `QuestionLang3` | `@LANGUAGE` section 2 | Language 2 translation (updated post-insert) |
| `QuestionLang4` | `@LANGUAGE` section 3 | Language 3 translation |
| `QuestionLang5` | `@LANGUAGE` section 4 | Language 4 translation |
| `QuestionLang6` | `@LANGUAGE` section 5 | Language 5 translation |
| `QuestionLang7` | `@LANGUAGE` section 6 | Language 6 translation |
| `QuestionLang8` | `@LANGUAGE` section 7 | Language 7 translation |
| `QuestionLang9` | `@LANGUAGE` section 8 | Language 8 translation |
| `QuestionLang10` | `@LANGUAGE` section 9 | Language 9 translation |

### QType Enumeration

| `QType` | Script keyword | Question type |
|---------|---------------|---------------|
| `1` | `*SR` | Single Response |
| `2` | `*MR` | Multiple Response |
| `3` | `*OPEN` | Open-ended (text) |
| `4` | `*NUMBER` | Numeric input |
| `5` | `*RANK` | Ranking (single) |
| `6` | `*IMAGE` | Image display |
| `7` | `*GRIDSR` | Grid Single Response |
| `8` | `*GRIDMR` | Grid Multiple Response |
| `9` | `*MEDIA` | Media player |
| `10` | `*RECORDING` | Audio recording |
| `12` | `*ALPHALIST` | Open text list |
| `13` | `*NUMLIST` | Numeric entry list |
| `14` | `*DATE` | Date input |
| `15` | `*TIME` | Time input |
| `16` | `*CAPTUREIMAGE` | Photo capture |
| `17` | `*NUMLISTTOTAL` | Numeric list with running total |
| `22` | `*AUTOCOMPLETE` / `*AUTOCOMPLETELIST` | Autocomplete dropdown |
| `23` | `*AUTOCOMPLETEANS` | Autocomplete answer display |
| `24` | `*DROPDOWN` / `*DROPDOWNLIST` | Dropdown select |
| `26` | `*DRAGDROP` | Drag-and-drop ranking |
| `27` | `*GRIDNUM` | Grid Numeric |
| `32` | `*PSCALE` | Point-scale grid |
| `40` | `*MAXDIFF` | MaxDiff exercise |
| `41` | `*GPS` | GPS coordinate capture |
| `48` | `*FORM` | Multi-field form |
| `49` | `*INFO` | Information/instruction screen |
| `50` | `*END` | Survey end screen |
| `51` | `*TERMINATE` | Screen-out / termination |
| `60` | `*FIFS` | Field Interviewer & Supervisor info |

---

## 3. `T_OptAttribute`

One row per option/attribute per question. Includes blank sentinel rows (filter with `WHERE QId <> ''`).

| Column | Type | Set by | Description |
|--------|------|--------|-------------|
| `ProjectId` | TEXT | `PROJECT CODE:` | Foreign key |
| `QId` | TEXT | `*QUESTION QId` | Parent question |
| `AttributeEnglish` | TEXT | Attribute label | English option label |
| `AttributeBengali` | TEXT | `@LANGUAGE` section 1 | Bengali translation (updated post-insert) |
| `AttributeValue` | TEXT | `N:label` prefix | Option code (the `N` in `N:label`) |
| `AttributeOrder` | INTEGER | Compiler | 1-based sequential position within the question |
| `TakeOpenended` | TEXT | `*OPEN` on attribute | `'1'` if this option triggers an open-end follow-up |
| `IsExclusive` | TEXT | `*NMUL` on attribute | `'1'` if selecting this option deselects all others |
| `LinkId1` | TEXT | Compiler | Type link code (see **LinkId1 Values** below) |
| `LinkId2` | TEXT | `*USELIST "name"` | Name of the referenced list when `LinkId1='1'` |
| `MinValue` | TEXT | `*MIN N` on attribute | Minimum numeric value for `*NUMLIST` fields |
| `MaxValue` | TEXT | `*MAX N` on attribute | Maximum numeric value |
| `ForceAndMsgOpt` | TEXT | `*MANDATORY` / FIFS | `'11'` = mandatory field; `''` otherwise |
| `GroupName` | TEXT | `*GROUPNAME "name"` | Rotation group name for grouped rotation |
| `FilterQid` | TEXT | `*INCLUDE [QId]` on attribute | Source question for attribute-level filtering |
| `FilterType` | TEXT | `*INCLUDE`/`*EXCLUDE` on attribute | `'1'`=Include, `'2'`=Exclude |
| `ExcepValue` | TEXT | `*EXCEPT [N;N]` | Attribute values to exclude from filter |
| `Comments` | TEXT | Attribute label | Same as `AttributeEnglish` by default |
| `AttributeLang3` | TEXT | `@LANGUAGE` section 2 | Language 2 translation |
| `AttributeLang4`–`AttributeLang10` | TEXT | `@LANGUAGE` sections 3–9 | Languages 3–9 translations |

### LinkId1 Values

| `LinkId1` | Meaning |
|-----------|---------|
| `''` | Standard option — no special link |
| `'1'` | This attribute row is a `*USELIST` placeholder; `LinkId2` holds the list name |
| `'3'` | FIFS system attribute (FI Name, FI Code, Supervisor Name, etc.) |

---

## 4. `T_GridInfo`

Grid column definitions. The `QId` column stores the **grid list name** (not the question QId). Matched to questions via `T_Question.AttributeId`. Includes one blank sentinel row per list.

| Column | Type | Description |
|--------|------|-------------|
| `ProjectId` | TEXT | Foreign key |
| `QId` | TEXT | Grid list name (matches `*GRIDLIST "name"`) |
| `AttributeEnglish` | TEXT | English column label |
| `AttributeBengali` | TEXT | Bengali translation (updated post-insert) |
| `AttributeValue` | TEXT | Column code (the `N` in `N:label`) |
| `AttributeOrder` | INTEGER | 1-based column position; used as match key for translation |
| `TakeOpenended` | TEXT | `'1'` if column triggers open-end; usually `''` |
| `IsExclusive` | TEXT | `'1'` if exclusive column; usually `''` |
| `MinValue` | TEXT | Minimum value for grid numeric columns |
| `MaxValue` | TEXT | Maximum value |
| `ForceAndMsgOpt` | TEXT | `'11'` = mandatory; `''` otherwise |
| `Comments` | TEXT | Same as `AttributeEnglish` |
| `AttributeLang3`–`AttributeLang10` | TEXT | Language translations (updated post-insert) |

> **Note:** Grid translation matching uses `QId` (list name) + `AttributeValue` (column code), not `AttributeOrder`. Grid rows in language sections must be in the same order as English because `AttributeOrder` is used in some grid contexts.

---

## 5. `T_LogicTable`

GOTO and MSG rules, plus question-level `*IF` conditions. Includes one blank sentinel row after each real row. Filter with `WHERE QId <> ''`.

| Column | Type | Description |
|--------|------|-------------|
| `ProjectId` | TEXT | Foreign key |
| `LogicId` | INTEGER | Sequential logic rule index (1-based, increments by 2 due to sentinel rows) |
| `QId` | TEXT | Question this logic is attached to |
| `LogicTypeId` | TEXT | See **LogicTypeId Values** below |
| `IfCondition` | TEXT | Condition expression, e.g. `Q1=1 AND Q2=2` |
| `Then` | TEXT | Action target — destination QId or message text |
| `Else` | TEXT | Always `''` (not used) |

### LogicTypeId Values (T_LogicTable)

| `LogicTypeId` | Script syntax | Meaning |
|---------------|--------------|---------|
| `'2'` | `*IF [...] *MSG "text"` | Show a validation message |
| `'3'` | `*IF [...] *GOTO QId` | Jump to another question |
| `'4'` | `*IF [...]` on `*QUESTION` line | Question-level visibility condition |

---

## 6. `T_LogicAuto`

INCLUDE and EXCLUDE auto-response rules. Same structure as `T_LogicTable` but `Then` encodes the filter action. Includes one blank sentinel row after each real row.

| Column | Type | Description |
|--------|------|-------------|
| `ProjectId` | TEXT | Foreign key |
| `LogicId` | INTEGER | Sequential index |
| `QId` | TEXT | Question whose attributes are filtered |
| `LogicTypeId` | TEXT | `''` (not explicitly set; app reads action from `Then`) |
| `IfCondition` | TEXT | Condition expression |
| `Then` | TEXT | Encoded filter action (see **Then Format** below) |
| `Else` | TEXT | Always `''` |

### Then Column Format (T_LogicAuto)

| Script syntax | `Then` value stored |
|---------------|---------------------|
| `*IF [...] *INCLUDE QId [1;2;3]` | `Include[1;2;3]` |
| `*IF [...] *EXCLUDE QId [1;2;3]` | `Exclude[1;2;3]` |
| `*IF [...] *INCLUDE QId [1 TO 5]` | `Include[1 TO 5]` |
| `*INCLUDE QId Q2` (standalone) | `Include[Q2]` |
| `*INCLUDE QId [1;2;3]` (standalone) | `Include[1;2;3]` |

---

## 7. `T_OptAttrbFilter`

Question-level attribute inheritance rules (`*INCLUDE [QId]` / `*EXCLUDE [QId]` on the `*QUESTION` header line). Includes one blank sentinel row after each real row.

| Column | Type | Description |
|--------|------|-------------|
| `ProjectId` | TEXT | Foreign key |
| `AttribFilterId` | INTEGER | Sequential index |
| `QId` | TEXT | Question that inherits attributes |
| `InheritedQId` | TEXT | Source question whose attributes are inherited |
| `FilterType` | TEXT | `'1'`=INCLUDE, `'2'`=EXCLUDE, `'5'`=INCLUDEBYORDER |
| `ExceptionalValue` | TEXT | Values excluded from inheritance (from `*EXCEPT`) |
| `LabelTakenFrom` | TEXT | `''` by default; may hold a label-source QId |

### FilterType Values

| `FilterType` | Script keyword | Behaviour |
|-------------|---------------|-----------|
| `'1'` | `*INCLUDE [QId]` on `*QUESTION` line | Inherit all attributes from `InheritedQId` |
| `'2'` | `*EXCLUDE [QId]` on `*QUESTION` line | Inherit everything *except* matching attributes |
| `'5'` | `*INCLUDEBYORDER [QId]` | Inherit by position order rather than by value code |

---

## 8. `T_LanguageMaster`

Pre-populated in the shell template; the compiler only issues UPDATEs, never INSERTs.

| Column | Type | Description |
|--------|------|-------------|
| `ProjectId` | INTEGER | Set to project code during compile |
| `LanguageName` | TEXT | Language display name (from `@LANGUAGE "name"` string) |
| `Status` | TEXT | `'1'` = active/enabled; `'2'` = inactive |
| `DisplayOrder` | INTEGER | `1`=English, `2`=Bengali, `3`=Lang3 … `10`=Lang10 |

**Update logic during compile:**

| Condition | Action |
|-----------|--------|
| `DisplayOrder=1` (English) | `Status='1'`, no `LanguageName` change |
| `DisplayOrder=2` (Bengali) | `Status='1'`, `LanguageName` = first `@LANGUAGE` name |
| `DisplayOrder=3`–`10` | `Status='1'`, `LanguageName` = corresponding `@LANGUAGE` name |
| Any `DisplayOrder > 1` with no matching `@LANGUAGE` section | `Status='2'` (inactive) |

---

## 9. Table Relationship Diagram

```
T_ProjectInfo
  │ ProjectId
  │
  ├─► T_Question          (ProjectId, QId)
  │       │
  │       ├─► T_OptAttribute    (ProjectId, QId)          — option rows
  │       ├─► T_GridInfo        (ProjectId, QId=ListName) — grid column rows
  │       │       ▲
  │       │       └── T_Question.AttributeId links to T_GridInfo.QId
  │       │
  │       ├─► T_LogicTable      (ProjectId, QId)          — GOTO/MSG/question-IF rules
  │       ├─► T_LogicAuto       (ProjectId, QId)          — INCLUDE/EXCLUDE rules
  │       └─► T_OptAttrbFilter  (ProjectId, QId)          — attribute inheritance rules
  │
  └─► T_LanguageMaster    (ProjectId, DisplayOrder)
```

---

## 10. Useful Query Patterns

### All questions in order
```sql
SELECT QId, QType, QuestionEnglish
FROM T_Question
WHERE QId <> ''
ORDER BY CAST(OrderTag AS INTEGER);
```

### All options for a question
```sql
SELECT AttributeValue, AttributeEnglish, AttributeBengali, TakeOpenended, IsExclusive
FROM T_OptAttribute
WHERE QId = 'Q1' AND QId <> ''
ORDER BY AttributeOrder;
```

### Grid columns for a grid list
```sql
SELECT AttributeValue, AttributeEnglish, AttributeOrder
FROM T_GridInfo
WHERE QId = 'ScaleList' AND QId <> ''
ORDER BY AttributeOrder;
```

### All GOTO rules
```sql
SELECT QId, IfCondition, [Then]
FROM T_LogicTable
WHERE LogicTypeId = '3' AND QId <> '';
```

### All MSG rules
```sql
SELECT QId, IfCondition, [Then]
FROM T_LogicTable
WHERE LogicTypeId = '2' AND QId <> '';
```

### All INCLUDE/EXCLUDE auto-response rules
```sql
SELECT QId, IfCondition, [Then]
FROM T_LogicAuto
WHERE QId <> '';
```

### All question-level attribute inheritance
```sql
SELECT QId, InheritedQId, FilterType, ExceptionalValue
FROM T_OptAttrbFilter
WHERE QId <> '';
```

### Questions with missing Bengali translation
```sql
SELECT QId, QuestionEnglish
FROM T_Question
WHERE QuestionEnglish <> '' AND (QuestionBengali IS NULL OR QuestionBengali = '')
ORDER BY CAST(OrderTag AS INTEGER);
```

### Active languages
```sql
SELECT DisplayOrder, LanguageName, Status
FROM T_LanguageMaster
WHERE Status = '1'
ORDER BY DisplayOrder;
```

---

## 11. Flag Value Reference

### Boolean-style flags (stored as TEXT `'0'`/`'1'`/`''`)

| Column | `'1'` means | `'0'` or `''` means |
|--------|------------|---------------------|
| `T_Question.HasAutoResponse` | Has `*INCLUDE`/`*EXCLUDE` auto rules | No auto rules |
| `T_Question.HasRandomAttrib` | Attribute rotation active (`*ROT`/`*RANDOM`) | Fixed order |
| `T_Question.HasRandomQntr` | Question itself rotates | Fixed position |
| `T_Question.HasMessageLogic` | Has `*IF [...] *MSG` rules | No message logic |
| `T_Question.HasMediaPath` | Has media (`*PICT`/`*VIDEO`) | No media |
| `T_Question.DisplayBackButton` | Back button shown | Back button hidden |
| `T_Question.DisplayNextButton` | Next button shown | Next button hidden |
| `T_Question.SilentRecording` | Silent audio recording on | Off |
| `T_OptAttribute.TakeOpenended` | Option triggers open-end follow-up | No open-end |
| `T_OptAttribute.IsExclusive` | Option is exclusive (`*NMUL`) | Not exclusive |

### `ForceAndMsgOpt` values

| Value | Meaning |
|-------|---------|
| `''` | No force/mandatory setting |
| `'11'` | Field is mandatory (from `*MANDATORY` on attribute, or system-set for FIFS fields) |
