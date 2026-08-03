# SmartSurvey Script — Logic System Reference

> **Scope:** This document covers all conditional logic and filtering directives in the SmartSurvey script language.
> Logic rules are written either **between questions** (standalone `*IF`, `*INCLUDE`, `*EXCLUDE`)
> or **inside a question header line** (question-level `*IF`).

---

## Overview of Logic Types

| Logic Type | Stored In | LogicTypeId / LogicId | Purpose |
|------------|-----------|----------------------|---------|
| `*IF [...] *GOTO` | `T_LogicTable` | LogicTypeId = `3` | Skip / jump to a target question |
| `*IF [...] *MSG` | `T_LogicTable` | LogicTypeId = `2` | Show a validation message |
| `*IF [...] *INCLUDE/EXCLUDE` | `T_LogicAuto` | LogicId = `1` | Dynamically filter attributes |
| `*QUESTION *IF [...]` | `T_LogicTable` | LogicTypeId = `4` | Conditionally show/hide a question |
| `*INCLUDE / *EXCLUDE` (standalone) | `T_LogicAuto` | LogicId = `1` | Always-on attribute filter |
| `*INCLUDE [QId]` (on question) | `T_OptAttrbFilter` | FilterType = `1` | Inherit options from another question |
| `*INCLUDEBYORDER [QId]` | `T_OptAttrbFilter` | FilterType = `5` | Inherit options by position order |
| `*EXCLUDE [QId]` (on question) | `T_OptAttrbFilter` | FilterType = `2` | Exclude options from another question |

---

## 1. Standalone `*IF` Block

**Position:** Between questions (not inside a `*QUESTION` line)

**General format:**
```
*IF [<condition>] *<ACTION>
```

The `*IF` line is split on `*` — exactly 3 parts are expected:
- Part 1: empty (before `*IF`)
- Part 2: `IF [<condition>]`
- Part 3: action (`GOTO ...`, `MSG "..."`, or `INCLUDE/EXCLUDE ...`)

---

### 1a. `*GOTO` — Skip Logic

**Stored in:** `T_LogicTable` (LogicTypeId = `3`)
**QId:** The question immediately before the `*IF` line (i.e., the question that was just parsed)

**Syntax:**
```
*IF [<condition>] *GOTO <TargetQId>
```

**Examples:**
```
*IF [Q1=2] *GOTO ScreenOut
*IF [Q3=1 OR Q3=2] *GOTO Q10
*IF [Q5>=18] *GOTO Q6
```

**Rules:**
- Target QId must be alphanumeric
- Target QId is not pre-validated for existence at parse time (written as-is to DB)
- Condition is validated by `CheckLogicalExp.checkIfCondition()`

**Database record:**
```
T_LogicTable: QId=<current_question>, ThenValue=<TargetQId>, LogicTypeId="3", IfCondition=<condition>
```

---

### 1b. `*MSG` — Validation Message

**Stored in:** `T_LogicTable` (LogicTypeId = `2`)
**QId:** The question immediately before the `*IF` line

**Syntax:**
```
*IF [<condition>] *MSG "<message text>"
```

**Examples:**
```
*IF [Q4<18] *MSG "Respondent must be 18 or older"
*IF [Q6=0] *MSG "Please enter a valid mobile number"
*IF [RegexOf[Q7]!=RegularExpOf[^\d{10}$]] *MSG "Mobile must be 10 digits"
```

**Rules:**
- Message text must be enclosed in double quotes
- Exactly one pair of double quotes required (`xyz.Length == 3` after split on `"`)
- `RegularExpOf[pattern]` is supported in the condition for regex-based validation (special parsing path)

**Database record:**
```
T_LogicTable: QId=<current_question>, ThenValue=<message_text>, LogicTypeId="2", IfCondition=<condition>
```

---

### 1c. `*INCLUDE` / `*EXCLUDE` inside `*IF`

**Stored in:** `T_LogicAuto` (LogicId = `1`)
**QId:** The target question whose attributes will be filtered

**Syntax — Form 1: Numeric list**
```
*IF [<condition>] *INCLUDE <TargetQId> [<val1>;<val2>;...]
*IF [<condition>] *EXCLUDE <TargetQId> [<val1>;<val2>;...]
```
```
*IF [Q1=1] *INCLUDE Q3Brand [1;3;5]
*IF [Q2!=3] *EXCLUDE Q4Cat [2;4]
```

**Syntax — Form 2: Range**
```
*IF [<condition>] *INCLUDE <TargetQId> [<low> TO <high>]
```
```
*IF [Q1=1] *INCLUDE Q3Brand [1 TO 5]
```

**Syntax — Form 3: Reference another question's answers**
```
*IF [<condition>] *INCLUDE <TargetQId> <SourceQId>
*IF [<condition>] *INCLUDE <TargetQId> <SourceQId.N>
```
```
*IF [Q1=1] *INCLUDE Q5Brands Q2SelectedBrands
*IF [Q3=1] *INCLUDE Q6Items Q4Items.3
```

**Syntax — Form 4: Function expression**
```
*IF [<condition>] *INCLUDE <TargetQId> <FUNCTION[...]>
```
See **Section 4 — Logic Functions** for all supported functions.

**Database record:**
```
T_LogicAuto: QId=<TargetQId>, ThenValue="Include[...]" or "Exclude[...]", LogicId="1", IfCondition=<condition>
```

---

## 2. Standalone `*INCLUDE` / `*EXCLUDE` (No Condition)

**Position:** Between questions (not inside `*QUESTION`)
**Stored in:** `T_LogicAuto` (LogicId = `1`)

These apply **always** (no conditional trigger) — they define a permanent filter on the target question's attributes.

**Syntax (same forms as conditional INCLUDE/EXCLUDE):**
```
*INCLUDE <TargetQId> [<val1>;<val2>;...]
*INCLUDE <TargetQId> [<low> TO <high>]
*INCLUDE <TargetQId> <SourceQId>
*INCLUDE <TargetQId> <FUNCTION[...]>

*EXCLUDE <TargetQId> [<val1>;<val2>;...]
*EXCLUDE <TargetQId> <SourceQId>
```

**Examples:**
```
*INCLUDE Q5Brands [1;2;3]
*EXCLUDE Q6Cat Q2SelCat
*INCLUDE Q8Items ASCRANKOf[Q7]
```

---

## 3. Question-Level `*IF` (Conditional Question Display)

**Position:** Inside the `*QUESTION` header line (alongside the type keyword)
**Stored in:** `T_LogicTable` (LogicTypeId = `4`)

**Syntax:**
```
*QUESTION <QId> *<TYPE> *IF [<condition>]
```

**Examples:**
```
*QUESTION Q5 *SR *IF [Q1=1]
*QUESTION Q10 *MR *IF [Q3=1 OR Q3=2]
*QUESTION Q15 *OPEN *IF [Q12=5 AND Q13>=3]
```

**Rules:**
- The entire question is shown only if the condition evaluates to true
- Condition validated via `checkIfCondition()`
- Stored with `ThenValue = QId` (the question conditionally shown)

**Database record:**
```
T_LogicTable: QId=<QId>, ThenValue=<QId>, LogicTypeId="4", IfCondition=<condition>
```

---

## 4. Question-Level Attribute Filters

These are written inside the `*QUESTION` header line and define a **permanent filter** that inherits or excludes options from a previously answered question.

### `*INCLUDE [QId]` — Inherit answered options

**FilterType:** `1`
**Stored in:** `T_OptAttrbFilter`

```
*QUESTION Q5 *MR *INCLUDE [Q3]
```
Only shows attributes from Q5 that the respondent selected/answered in Q3.

---

### `*INCLUDEBYORDER [QId]` — Inherit by position order

**FilterType:** `5`
**Stored in:** `T_OptAttrbFilter`

```
*QUESTION Q6 *SR *INCLUDEBYORDER [Q4]
```
Includes attributes in the same ordinal position as selections made in Q4.

---

### `*EXCLUDE [QId]` — Exclude answered options

**FilterType:** `2`
**Stored in:** `T_OptAttrbFilter`

```
*QUESTION Q7 *MR *EXCLUDE [Q3]
```
Hides from Q7 any options that were selected in Q3.

---

### `*INCLUDEGRIDLIST [QId]`

**Stored in:** Grid attribute `FilterQid` / `FilterType`

```
*QUESTION Q8 *GRIDSR *USEGRIDLIST "ScaleList" *INCLUDEGRIDLIST [Q4]
```
Filters the grid rows based on selections in Q4 (written to each grid attribute's `FilterQid`/`FilterType` fields).

---

## 5. Condition Syntax

### Basic Comparisons

| Operator | Meaning | Example |
|----------|---------|---------|
| `=` | Equals | `Q1=1` |
| `!=` | Not equal | `Q1!=3` |
| `<` | Less than | `Q4<18` |
| `>` | Greater than | `Q4>65` |
| `<=` | Less than or equal | `Q4<=17` |
| `>=` | Greater than or equal | `Q4>=18` |

### Logical Combiners

| Operator | Meaning | Example |
|----------|---------|---------|
| `AND` | Both must be true | `Q1=1 AND Q2=2` |
| `OR` | Either must be true | `Q1=1 OR Q1=2` |

**Example:**
```
*IF [Q3=1 OR Q3=2 AND Q4>=18] *GOTO Q6
```

### Value List in Conditions

Use semicolons to check multiple values:
```
*IF [Q1=1;2;3] *GOTO Q6
```

### Regex Validation Condition

A special condition path used **only with `*MSG`**:

```
*IF [RegexOf[<QId>]!=RegularExpOf[<pattern>]] *MSG "<error text>"
*IF [RegexOf[<QId>.<position>]!=RegularExpOf[<pattern>]] *MSG "<error text>"
```

**Examples:**
```
*IF [RegexOf[Q6]!=RegularExpOf[^\d{11}$]] *MSG "Mobile must be 11 digits"
*IF [RegexOf[Q17.1]!=RegularExpOf[^[1-9]\d*$]] *MSG "Must be a positive number"
```

**Notes:**
- The `*` in `RegularExpOf` requires special 4-part splitting in the parser
- Only `*MSG` action is supported with regex conditions (not `*GOTO` or `*INCLUDE`)

---

## 6. Logic Functions (for INCLUDE/EXCLUDE values)

These functions are used as the **value source** in `*INCLUDE` / `*EXCLUDE` directives.
They are resolved at runtime on the device, not during compile time.

---

### Ranking Functions

#### `ASCRANKOf[QId]`
Returns the attribute values of `QId` sorted in **ascending** rank order.
```
*INCLUDE Q5Brands ASCRANKOf[Q4RankBrands]
```

#### `DSCRANKOf[QId]`
Returns the attribute values of `QId` sorted in **descending** rank order.
```
*INCLUDE Q5Brands DSCRANKOf[Q4RankBrands]
```

---

### Splitting & Sampling Functions

#### `SPLITOf[QId,position]`
Splits the responses of `QId` at the given `position` and returns that portion.
```
*INCLUDE Q6 SPLITOf[Q3BrandList,2]
```

#### `RANDOMVALUEOf[QId,count]`
Randomly picks `count` attribute values from the responses of `QId`.
```
*INCLUDE Q5 RANDOMVALUEOf[Q3Selected,3]
```

#### `RANBETWEENOf[low,high]`
Returns a random integer between `low` and `high` (inclusive).
```
*INCLUDE Q7 RANBETWEENOf[1,5]
```

#### `VALUEINDEXOf[QId.position]`
Returns the value at `position` in the answer list of `QId`.
```
*INCLUDE Q8 VALUEINDEXOf[Q5.2]
```

#### `VALUEINPOSITIONOf[QId,position]`
Similar to `VALUEINDEXOf` — returns the value at a specific position index.
Supports negative positions.
```
*INCLUDE Q9 VALUEINPOSITIONOf[Q6,1]
```

#### `SUBSTROf[QId.start,length,count]` or `SUBSTROf[QId,start,length]`
Extracts a substring from the response value of `QId`.
```
*INCLUDE Q10 SUBSTROf[Q7.1,3,2]
```

---

### Math Functions

#### `SUMOf[QId]` or `SUMOf[QId.pos1,QId.pos2,...]`
Returns the sum of numeric responses from `QId` (single or multiple positions).
```
*IF [SUMOf[Q13]!=100] *MSG "Total must equal 100"
*INCLUDE Q14 SUMOf[Q10.1,Q11.1,Q12.1]
```

#### `SUBTRACTOf[QId,value]` or `SUBTRACTOf[QId1,QId2]`
Subtracts `value` (or QId2's value) from `QId`'s response.
```
*INCLUDE Q15 SUBTRACTOf[Q12,Q11]
```

#### `MULTIPLYOf[QId,value]` or `MULTIPLYOf[QId1,QId2]`
Multiplies `QId`'s response by `value` or by another question's response.
```
*INCLUDE Q16 MULTIPLYOf[Q9,12]
```

#### `DIVIDEOf[QId,value]` or `DIVIDEOf[QId1,QId2]`
Divides `QId`'s response by `value` or another question's response.
```
*INCLUDE Q17 DIVIDEOf[Q13,Q12]
```

#### `TOTALOf[QId]`
Returns the total/count of responses recorded for `QId` (used with grid types).
```
*IF [TOTALOf[Q11]<2] *MSG "Please select at least 2 items"
```

---

### Date & Time Functions

#### `DATEOf[TODAY]`
Returns today's date. Used in date comparisons.
```
*IF [Q14<=DATEOf[TODAY]] *MSG "Date cannot be in the future"
```

#### `DAYOf[TODAY]`
Returns today's day of week (or day number).
```
*INCLUDE Q18 DAYOf[TODAY]
```

#### `TIMEOf[NOW]`
Returns the current time.
```
*IF [TIMEOf[NOW]<0800] *MSG "Survey only available after 8 AM"
```

#### `TIMEDIFFOf[QId1,QId2]`
Calculates the time difference between two time questions.
```
*IF [TIMEDIFFOf[Q15Start,Q16End]<30] *MSG "Duration too short"
```

---

### Interview / Session Functions

#### `TYPEOf[INTERVIEW]`
Returns the interview type (CAPI, CATI, etc.).
```
*INCLUDE Q20 TYPEOf[INTERVIEW]
```

#### `LANGUAGEOf[INTERVIEW]`
Returns the current interview language code.
```
*INCLUDE Q21 LANGUAGEOf[INTERVIEW]
```

#### `NUMBEROFOf[INTERVIEW]`
Returns the count of interviews for the current session.

**Variants:**
```
NUMBEROFOf[INTERVIEW]
NUMBEROFOf[INTERVIEW,QId]
NUMBEROFOf[INTERVIEW,QId1,QId2]
```
```
*IF [NUMBEROFOf[INTERVIEW]>5] *GOTO QuotaFull
```

#### `USERIDOf[INTERVIEW]`
Returns the interviewer/user ID from the session.
```
*INCLUDE Q22 USERIDOf[INTERVIEW]
```

---

### Location Functions

#### `DISTANCEFROMOf[QId]`
Returns the distance from the GPS coordinates captured in `QId`.
```
*IF [DISTANCEFROMOf[Q41GPS]>500] *MSG "You are too far from the target location"
```

#### `DISTANCEBTNOf[QId1,QId2]`
Returns the distance between two GPS-captured locations.
```
*IF [DISTANCEBTNOf[Q41Start,Q42End]>1000] *MSG "Distance exceeds limit"
```

---

### Data Lookup Functions

#### `PANELINFOOf[QId,field]`
Retrieves a field value from the panel/respondent database linked to `QId`.
```
*INCLUDE Q23 PANELINFOOf[Q1PanelId,3]
```

#### `POSTCODEVALUEOf[QId,position]`
Looks up a value from a postcode table using the answer to `QId`.
```
*INCLUDE Q24 POSTCODEVALUEOf[Q5PostCode,2]
```

---

### String Functions

#### `STRINGOf[QId,value]` or `STRINGOf[QId1,QId2]`
Performs a string operation on `QId`'s response value.
```
*INCLUDE Q25 STRINGOf[Q7Name,1]
```

---

### Regex Function (in conditions only)

#### `RegularExpOf[pattern]` with `RegexOf[QId]`
Validates a response against a regular expression.
Only valid in `*IF [...] *MSG` statements.
```
*IF [RegexOf[Q6]!=RegularExpOf[^\d{11}$]] *MSG "Enter a valid 11-digit number"
*IF [RegexOf[Q17.1]!=RegularExpOf[^[1-9]\d*$]] *MSG "Must be a positive integer"
```

---

## 7. Recording Blocks

Used to flag questions for silent/background audio recording.

### `*STARTREC "FieldName"`
Begins a silent recording scope. All questions following this directive (until `*ENDREC`) will have their `SilentRecording` field set to `"FieldName"`.

```
*STARTREC "BackgroundAudio"
*QUESTION Q5 *OPEN
Please describe your opinion.
*ENDREC
```

### `*ENDREC`
Ends the silent recording scope.

**Rules:**
- `*STARTREC "FieldName"` — FieldName must be enclosed in double quotes
- `*ENDREC` — must be exactly 7 characters (no trailing text)

---

## 8. Complete Logic Syntax Examples

```
# Skip to screen-out if age < 18
*IF [Q_Age<18] *GOTO TermAge

# Show message if mobile number is wrong length
*IF [RegexOf[Q_Mobile]!=RegularExpOf[^\d{11}$]] *MSG "Please enter a valid 11-digit mobile number"

# Conditionally filter brands based on category selection
*IF [Q_Category=1] *INCLUDE Q_Brands [1;2;3;4]
*IF [Q_Category=2] *INCLUDE Q_Brands [5;6;7;8]

# Show next question only if respondent selected brand 1 or 2
*QUESTION Q_BrandFeedback *SR *IF [Q_Brands=1 OR Q_Brands=2]
How satisfied are you with this brand?
1:Very Satisfied
2:Satisfied
3:Neutral
4:Dissatisfied
5:Very Dissatisfied

# Rotate brands in awareness question based on ranked usage
*INCLUDE Q_Awareness ASCRANKOf[Q_UsageRank]

# Always exclude brands already mentioned in Q3 from Q5
*EXCLUDE Q_Spontaneous Q_Prompted

# Validate that total spend equals 100
*IF [SUMOf[Q_Spend]!=100] *MSG "Values must add up to 100"
```

---

## 9. Logic Storage Summary

### T_LogicTable
Stores `*GOTO`, `*MSG`, and question-level `*IF` rules.

| Column | Values |
|--------|--------|
| `ProjectId` | From PROJECT CODE |
| `QId` | Question the logic is attached to |
| `ThenValue` | Target QId (GOTO) or message text (MSG) or own QId (question IF) |
| `LogicTypeId` | `2`=MSG, `3`=GOTO, `4`=Question IF |
| `IfCondition` | The raw condition expression |

### T_LogicAuto
Stores `*INCLUDE` / `*EXCLUDE` filter rules (both conditional and unconditional).

| Column | Values |
|--------|--------|
| `ProjectId` | From PROJECT CODE |
| `QId` | Target question being filtered |
| `ThenValue` | `"Include[...]"` or `"Exclude[...]"` with value/function |
| `LogicId` | Always `"1"` |
| `IfCondition` | Condition (empty for standalone INCLUDE/EXCLUDE) |

### T_OptAttrbFilter
Stores question-level `*INCLUDE [QId]`, `*INCLUDEBYORDER [QId]`, `*EXCLUDE [QId]`.

| Column | Values |
|--------|--------|
| `ProjectId` | From PROJECT CODE |
| `QId` | The question being filtered |
| `InheritedQId` | The source question to inherit/exclude from |
| `FilterType` | `1`=INCLUDE, `5`=INCLUDEBYORDER, `2`=EXCLUDE |
