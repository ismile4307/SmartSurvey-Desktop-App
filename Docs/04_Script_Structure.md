# SmartSurvey Script — Structure & Layout Reference

> **Scope:** This document describes the physical layout rules of a `.q` script file —
> how it is organised top-to-bottom, what each section contains, how lines are parsed,
> and the ordering rules that the compiler enforces or assumes.

---

## 1. File Format Basics

| Property | Value |
|----------|-------|
| File extension | `.q` |
| Encoding | Plain text (UTF-8 or ANSI) |
| Line endings | Windows (`CRLF`) or Unix (`LF`) — both accepted |
| Case sensitivity | **Case-insensitive** for all keywords (`*SR` = `*sr` = `*Sr`) |
| Whitespace | Leading/trailing whitespace is trimmed; multiple spaces collapsed to one |

---

## 2. Comment Lines

A line is **completely ignored** by the compiler if its first non-whitespace character is `#` or `$`.

```
# This is a comment — ignored by the compiler
$ This is also a comment
######################################
# Section: Awareness
######################################
```

**Rules:**
- The `#` or `$` must be the **very first character** of the line (after trimming)
- Inline comments (at the end of a data line) are **not supported**
- A comment on an attribute line or question text line disables that entire line
- Use `#` freely for section headers, version notes, disabled questions

**Example of disabling a question:**
```
*QUESTION Q5 *SR
Which pack size do you prefer?
1:Small
2:Medium
3:Large

# The following question is under review — disabled
#*QUESTION Q6 *SR
#How often do you purchase?
#1:Daily
#2:Weekly
```

---

## 3. Blank Lines

Blank lines (empty or whitespace-only) are **silently skipped** by the compiler.
They have no effect on parsing and can be used freely for readability.

---

## 4. Top-Level Script Sections

A `.q` file is divided into these sections in order:

```
┌─────────────────────────────┐
│  1. FILE HEADER             │  (Project metadata — 5 required fields)
├─────────────────────────────┤
│  2. ENGLISH BODY            │  (Lists, Grid Lists, Questions, Logic)
├─────────────────────────────┤
│  @LANGUAGE "Language1"      │  (Language section marker)
│  3. LANGUAGE 1 BODY         │  (Translated lists and questions)
├─────────────────────────────┤
│  @LANGUAGE "Language2"      │
│  4. LANGUAGE 2 BODY         │
├─────────────────────────────┤
│  ...up to 9 language        │
│  sections supported...      │
└─────────────────────────────┘
```

**Key rule:** The `@LANGUAGE` line acts as a **hard section separator**.
Everything before the first `@LANGUAGE` is the English body.
Everything after belongs to the named language section.

---

## 5. Section 1 — File Header

The header provides project metadata. These five fields are required and can appear in any order, but conventionally appear at the top of the file.

```
Project Name : Eagle (April 26)
Project Code : 261701
Script Version : 1.0.0.7
Script Name : KNI21858
Scripted By : Md Habibur Rahman
```

### Header Fields

| Field | Required | Stored In | Notes |
|-------|----------|-----------|-------|
| `Project Name :` | Yes | `T_ProjectInfo.ProjectName` | Free text after `:` |
| `Project Code :` | Yes | `T_ProjectInfo.ProjectId` | Used as primary key in all tables |
| `Script Version :` | Yes | `T_ProjectInfo.Version` | Free text (e.g. `1.0.0.3`) |
| `Script Name :` | Yes | `T_ProjectInfo.JobNo` / output `.db` filename | Used to name the output database |
| `Scripted By :` | Yes | Not stored in DB | Compiler reads but does not validate content |

**Parsing rule:** The compiler checks for the keyword (`PROJECT NAME`, etc.) anywhere on the line
and splits on `:` to get the value. The field name is case-insensitive.

**Error messages if missing:**
```
Line : 3 Project Name Missing
Line : 4 Project Code Missing
Line : 5 Script Version Missing
Line : 6 Script Name Missing
Line : 8 Scripted by name Missing
```

**Optional extra fields** (not parsed by the compiler, treated as comments):
```
Date : 04.05.2026
Client : XYZ Corp
```
These lines are silently skipped because they don't match any header keyword.

---

## 6. Section 2 — English Body

The English body is the main content of the script. It contains four types of elements
that can be **freely interleaved** in any order, with these constraints:

> **Critical ordering rule:** A `*LIST` or `*GRIDLIST` must appear **before** any
> `*QUESTION` or `*IF` that references it by name.

### 6.1 Element Types

| Element | Starts With | Purpose |
|---------|-------------|---------|
| `*LIST` block | `*LIST "name"` | Define a reusable option list |
| `*GRIDLIST` block | `*GRIDLIST "name"` | Define a reusable grid column list |
| `*QUESTION` block | `*QUESTION QId *TYPE ...` | Define a survey question |
| Logic directive | `*IF`, `*INCLUDE`, `*EXCLUDE` | Define conditional or filter rules |
| `*STARTREC` / `*ENDREC` | `*STARTREC "field"` | Recording scope markers |
| `*REPEAT` block | `*REPEAT [source]` | Loop-expand questions over a list |
| Comment line | `#` or `$` | Ignored |
| Blank line | (empty) | Ignored |

---

### 6.2 `*LIST` Block

Defines a named, reusable list of attribute options. Must appear **before** any question
that uses it via `*USELIST "name"`.

```
*LIST "name"
value:label
value:label *OPEN
value:label *NMUL
value:label *MANDATORY
value:label *ALPHA
value:label *NUMBER *MIN 0 *MAX 100
```

**Rules:**
- Name must be enclosed in double quotes: `*LIST "BrandList"`
- First line after `*LIST` must be an attribute line (`value:label`)
- Block ends when a line starts with `*` (next directive)
- Values must be numeric
- Values must be unique within the list
- Labels must be unique (case-insensitive) within the list
- Attribute modifiers on list items: `*OPEN`, `*NMUL`, `*ALPHA`, `*NUMBER`, `*MANDATORY`, `*MIN n`, `*MAX n`, `*PICT "path"`, `*VIDEO "path"`

**Example:**
```
*LIST "YesNo"
1:Yes
2:No

*LIST "AgreementScale"
1:Strongly Agree
2:Agree
3:Neutral
4:Disagree
5:Strongly Disagree
```

---

### 6.3 `*GRIDLIST` Block

Defines a named list of **grid column headers** (the response scale for grid questions).
Must appear before any question that uses it via `*USEGRIDLIST "name"`.

```
*GRIDLIST "name"
value:label
value:label *OPEN
value:label *NMUL
value:label *MANDATORY
value:label *PICT "path"
value:label *VIDEO "path"
value:label *MIN n *MAX n
```

**Rules:**
- Same format as `*LIST` but stored separately in `dicGridListNameVsList`
- Duplicate grid list names are rejected
- Block ends when a line starts with `*`

**Example:**
```
*GRIDLIST "Scale5"
1:Strongly Agree
2:Agree
3:Neither Agree nor Disagree
4:Disagree
5:Strongly Disagree

*GRIDLIST "WeekDays"
1:Monday
2:Tuesday
3:Wednesday
4:Thursday
5:Friday
6:Saturday
7:Sunday
```

---

### 6.4 `*QUESTION` Block

Defines a single survey question. Each block has three parts: header line, question text, and attributes.

#### Part A — Header Line

```
*QUESTION <QId> *<TYPE> [*MODIFIER ...] [*IF [condition]]
```

- Everything on **one single line**
- Split on `*` to extract individual tokens
- Unknown tokens cause an `Invalid Token` error (validated against `listOfKeyWords`)
- Duplicate tokens on the same line cause a `Duplicate Token` error
- Exactly one type keyword is required; multiple type keywords cause a `QTypeCounter > 1` conflict

#### Part B — Question Text

- Begins on the line **immediately after** the header
- Can span **multiple lines** — each line is joined with `<br>`
- Ends when a line starts with `*` or is a numeric attribute (`value:label`)
- Cannot be empty — an empty question text causes an error
- Supports `{QId}` and `{QId.N}` piping syntax (validated against previously defined QIds)
- Supports HTML tags (e.g. `<font color="#F5AFA3">...</font>`, `<b>`, `<br>`)

#### Part C — Attributes

- Each attribute is on its own line: `value:label [*MODIFIER ...]`
- Value must be a positive integer
- Values must be unique within the question
- Labels must be unique within the question (case-insensitive)
- Block ends when a line starts with `*` (next directive or next `*QUESTION`)
- Can be replaced by `*USELIST "name"` to import a predefined list

**Full example:**
```
*QUESTION Q1 *SR *ROT
Q1. Which brand of carbonated soft drink do you know of?
    (CODE FIRST MENTION)
1:Coca-Cola
2:Sprite
3:Pepsi
4:7UP
95:Other *OPEN
99:None *NMUL
```

**Attribute Modifiers (inline on each attribute line):**

| Modifier | Effect |
|----------|--------|
| `*OPEN` | This option triggers a free-text entry field |
| `*NMUL` | Mutually exclusive — selecting this deselects others |
| `*NOCON` | No consolidation in reporting |
| `*MANDATORY` | This field must be filled in |
| `*MIN n` | Minimum value (numeric inputs) |
| `*MAX n` | Maximum value (numeric inputs) |
| `*PICT "path"` | Display an image with this option |
| `*VIDEO "path"` | Play a video with this option |
| `*ALPHA` | Accept text input for this row (in lists/forms) |
| `*NUMBER` | Accept numeric input for this row (in lists/forms) |
| `*SR` | Single response sub-type (in `*FORM`) |
| `*MR` | Multiple response sub-type (in `*FORM`) |
| `*DROPDOWN` | Dropdown sub-type (in `*FORM`) |
| `*AUTOCOMPLETE` | Autocomplete sub-type (in `*FORM`) |
| `*DATE` | Date sub-type (in `*FORM`) |
| `*TIME` | Time sub-type (in `*FORM`) |
| `*USEGRIDLIST "name"` | Link this row to a grid list (in `*FORM`) |
| `*LAT` | Marks this attribute as latitude capture (`*GPS`) |
| `*LON` | Marks this attribute as longitude capture (`*GPS`) |
| `*COMPVAL n` | Comparison value for this attribute |
| `*EXCEPT value` | Exception value |
| `*GROUPNAME "label"` | Assigns this attribute to a named group |
| `*GROUPHEAD` | Marks this attribute as a group header |

---

### 6.5 Logic Directives (between questions)

Logic directives appear **between** `*QUESTION` blocks (not inside them).
Each is a single line.

```
*IF [condition] *GOTO TargetQId
*IF [condition] *MSG "message text"
*IF [condition] *INCLUDE TargetQId [val1;val2;val3]
*IF [condition] *EXCLUDE TargetQId SourceQId

*INCLUDE TargetQId [val1;val2;val3]
*EXCLUDE TargetQId SourceQId
```

**Ordering rule:** Logic directives that reference a QId (`*INCLUDE Q3 Q1`) require
that the **source** QId (`Q1`) has already been defined above.
The **target** QId (`Q3`) should also be defined before being referenced.

---

### 6.6 Recording Scope (`*STARTREC` / `*ENDREC`)

Wraps questions in a silent background recording block.

```
*STARTREC "AudioField1"

*QUESTION Q5 *OPEN
Please describe your experience.

*QUESTION Q6 *MR
Which features did you notice?
1:Feature A
2:Feature B

*ENDREC
```

**Rules:**
- `*STARTREC "fieldname"` — field name in double quotes, no other content
- `*ENDREC` — must be exactly 7 characters (the word `*ENDREC` only, no trailing text)
- All questions between these markers get `SilentRecording = "fieldname"` set
- Can appear anywhere in the English body

---

### 6.7 `*REPEAT` Block

Expands a template of questions (or lists) once for each item in a source list or question.

```
*REPEAT [SourceName]
  ... template lines ...
*ENDREPEAT
```

**Format:**
```
*REPEAT [ListNameOrQId]
*QUESTION {ITEM}Attr *SR
Please rate {ITEM}
1:Very Good
2:Good
3:Average
4:Poor
*ENDREPEAT
```

**Source types for `[SourceName]`:**
- A `*LIST` name defined earlier in the script
- A `*QUESTION` QId whose attributes will be iterated

**Rules:**
- The `[source]` brackets are required — `*REPEAT` without `[...]` is an error
- Must be closed with `*ENDREPEAT`
- An unclosed `*REPEAT` logs: `*REPEAT block not closed with *ENDREPEAT`
- Source must be defined **before** the `*REPEAT` block

**Inline `*REPEAT` on a `*QUESTION` line** (attribute-level repeat):
```
*QUESTION QId *SR *REPEAT [SourceQId]
What do you think about {item}?
1:Good
2:Bad
```
This creates one separate question per attribute of `SourceQId`, using the attribute label
as the `{item}` substitution. If an attribute contains `"None"` it is skipped.

---

## 7. `*USELIST` and `*USEGRIDLIST`

These two directives replace inline attribute blocks with a pre-defined list.

### `*USELIST "name"`

Appears as the **first attribute line** (instead of `value:label` lines) in a `*QUESTION` block.
Imports all attributes from the named `*LIST`.

```
*QUESTION Q3 *MR *ROT
Q3. Which brands have you ever tried?
*USELIST "BrandList"
```

**Rules:**
- `*USELIST` is detected by checking if the first attribute line matches `*USELIST`
- The list name must exactly match a previously defined `*LIST "name"`
- For grid question types (`*GRIDSR`, `*GRIDMR`, `*PSCALE`, `*DRAGDROP`),
  `*USEGRIDLIST` must also be specified on the header line
- Cannot be combined with inline attribute lines on the same question

### `*USEGRIDLIST "name"`

Appears in the **header line** of a grid question.
References the columns (scale) for that grid.

```
*QUESTION Q7 *GRIDSR *USEGRIDLIST "Scale5"
Please rate each brand on the following.
1:Brand A
2:Brand B
3:Brand C
```

**Rules:**
- Name must match a previously defined `*GRIDLIST "name"`
- Required for: `*GRIDSR`, `*GRIDMR`, `*GRIDNUM`, `*PSCALE`, `*DRAGDROP`
- Also used with `*DROPDOWN` / `*AUTOCOMPLETE` (single-dropdown mode)

---

## 8. Section 3+ — Language Sections

Additional language translations are added after the English body using `@LANGUAGE` markers.

```
@LANGUAGE "Bengali"
[Bengali translations...]

@LANGUAGE "Hindi"
[Hindi translations...]
```

### Structure of a language section

Each language section mirrors the English body in structure but only contains:
- `*LIST "name"` blocks with translated labels
- `*GRIDLIST "name"` blocks with translated labels
- `*QUESTION QId` blocks with translated text and attribute labels
- `*REPEAT` blocks (if used in English, must be mirrored in each language)

Logic directives (`*IF`, `*GOTO`, etc.) are **not repeated** in language sections —
logic is defined only once in the English section.

```
@LANGUAGE "Bengali"

*LIST "YesNo"
1:হ্যাঁ
2:না

*QUESTION Q1 *SR
প্রশ্ন ১. আপনি কোন ব্র্যান্ডের কথা প্রথমে মনে করেন?
1:কোকা-কোলা
2:স্প্রাইট
3:পেপসি
```

### Language Matching Rules

The language parser matches translations to English entries **by QId**.
- If a QId exists in English but not in a language section, the English text is used as fallback
- If a QId exists in a language section but not in English, it is ignored
- Attribute values must match exactly (the same numeric codes)
- Attribute order within the language section should match English

### Language → Database Field Mapping

| Language section | DB Field (Question) | DB Field (Attribute) |
|-----------------|---------------------|----------------------|
| Language 1 (first `@LANGUAGE`) | `QuestionLang3` | `AttributeLang3` |
| Language 2 | `QuestionLang4` | `AttributeLang4` |
| Language 3 | `QuestionLang5` | `AttributeLang5` |
| Language 4 | `QuestionLang6` | `AttributeLang6` |
| Language 5 | `QuestionLang7` | `AttributeLang7` |
| Language 6 | `QuestionLang8` | `AttributeLang8` |
| Language 7 | `QuestionLang9` | `AttributeLang9` |
| Language 8 | `QuestionLang10` | `AttributeLang10` |
| Language 9 | (10th language) | (10th language) |

> **Note:** Lang1 (English) → `QuestionEnglish` / `AttributeEnglish`.
> Lang2 (Bengali/first `@LANGUAGE`) → `QuestionLang3` (field name skips Lang2 for historical reasons).

---

## 9. Complete Valid Keyword List

Any token starting with `*` inside a `*QUESTION` header must be in this list,
or it triggers `Invalid Token` in `BuildResult.txt`.

**Question Types:**
`QUESTION`, `SR`, `MR`, `ALPHA`, `NUMBER`, `RANK`, `IMAGE`, `GRIDSR`, `GRIDMR`,
`GRIDNUM`, `MEDIA`, `ALPHALIST`, `NUMLIST`, `DATE`, `TIME`, `CAPTUREIMAGE`,
`NUMLISTTOTAL`, `AUTOCOMPLETE`, `AUTOCOMPLETELIST`, `AUTOCOMPLETEANS`, `DROPDOWN`,
`DROPDOWNLIST`, `DRAGDROP`, `FORM`, `INFO`, `MAXDIFF`, `GPS`, `RECORDING`,
`PSCALE`, `END`, `TERMINATE`, `FIFS`

**Rotation / Randomisation:**
`RANDOM`, `ROT`, `QROT`, `FROT`, `GROUPROT`, `OTPGROUPROT`, `OTPROTGROUP`, `OTPROTGROUPROT`,
`GROT`, `GRANDOM`

**Layout / Display:**
`COLUMN`, `HORIZONTAL`, `TAKEONLYONE`, `SHOWASFORM`, `ADDSEARCH`, `FONTSIZE`,
`DIRIMAGE`, `SHOWASNUMTEXT`, `BLOCK`, `JUMPFOR`, `DELAY`, `IMGADJBY`,
`NOBACKBTN`, `NONEXTBTN`, `ADDRESS1`, `ADDRESS2`, `ADDRESS3`, `ADDRESS4`, `TBC`

**Response Control:**
`MIN`, `MAX`, `MANDATORY`, `OPEN`, `NMUL`, `NOCON`, `DKCS`, `DUMMY1`, `DUMMY2`,
`EXCEPT`, `EXTCAMERA`

**Filtering / Logic:**
`IF`, `INCLUDE`, `INCLUDEBYORDER`, `EXCLUDE`, `INCLUDEGRIDLIST`, `FILTER`, `GOTO`

**Media / Content:**
`PICT`, `VIDEO`, `QLABEL`

**Lists:**
`LIST`, `USELIST`, `GRIDLIST`, `USEGRIDLIST`

**Repeat:**
`REPEAT`, `ENDREPEAT`

**Recording:**
`STARTREC`, `ENDREC`

**Special Attributes:**
`LAT`, `LON`, `COMPVAL`, `GROUPNAME`, `INRLD`, `USERIDOF`

---

## 10. Parsing Rules Summary

| Rule | Detail |
|------|--------|
| Line cleaning | Each line is trimmed and multiple spaces collapsed before processing |
| Comment removal | Lines starting with `#` or `$` are removed entirely |
| Blank line removal | Empty lines are skipped |
| Language split | First `@LANGUAGE` line breaks the English section; subsequent ones create new language sections |
| `*` directives | Any line whose first character (after trim) is `*` is a directive |
| Attribute lines | Lines in `value:label` format; value must be numeric |
| Question text lines | Any line that is neither a directive nor an attribute, after the question header |
| Line numbering | Physical file line number is tracked for all error messages; blank/comment lines count toward physical line count |

---

## 11. Full Annotated Script Template

```
######################################################################
# PROJECT HEADER
######################################################################

Project Name  : My Research Study
Project Code  : 999001
Script Version: 1.0.0.1
Script Name   : MRS999001DB
Scripted By   : Your Name

######################################################################
# SHARED LISTS — define before any question that uses them
######################################################################

*LIST "BrandList"
1:Brand A
2:Brand B
3:Brand C
4:Brand D
99:None of the above *NMUL

*GRIDLIST "AgreementScale"
1:Strongly Agree
2:Agree
3:Neutral
4:Disagree
5:Strongly Disagree

######################################################################
# SYSTEM / FIELD MANAGEMENT QUESTIONS
######################################################################

*QUESTION FIFSInfo *FIFS
Field Interviewer and Supervisor Details

*QUESTION RespName *OPEN
Respondent Name

*QUESTION RespMobile *NUMBER
Respondent Mobile Number

*QUESTION Centre *SR
Select Centre
1:City A
2:City B
3:City C

######################################################################
# SCREENING
######################################################################

*QUESTION S1 *SR
S1. Are you the primary grocery shopper in your household?
1:Yes
2:No

*IF [S1=2] *GOTO ScreenOut

*QUESTION S2 *MR *MIN 1 *MAX 3
S2. Which of the following categories have you purchased in the last 3 months?
1:Beverages
2:Snacks
3:Personal Care
4:Household
99:None *NMUL

*IF [S2=99] *GOTO ScreenOut

######################################################################
# MAIN QUESTIONNAIRE
######################################################################

# Dummy question for pre-selecting brands (DUMMY1 = auto-fill from logic)
*QUESTION Q1Dummy *MR *DUMMY1
Brand Filter Dummy
*USELIST "BrandList"

*IF [S2=1] *INCLUDE Q1Dummy [1;2;3]
*IF [S2=2] *INCLUDE Q1Dummy [2;3;4]

*QUESTION Q1 *SR *INCLUDE [Q1Dummy]
Q1. Which brand of beverage do you drink most often?
*USELIST "BrandList"

*QUESTION Q2 *MR *EXCLUDE [Q1] *MAX 3
Q2. Which other brands have you tried?
*USELIST "BrandList"

*QUESTION Q3 *GRIDSR *USEGRIDLIST "AgreementScale"
Q3. Please rate your agreement with the following statements about {Q1}.
1:It tastes great
2:It is good value for money
3:It is widely available
4:I would recommend it to others

*QUESTION Q4 *OPEN
Q4. In your own words, why is {Q1} your favourite brand?

*QUESTION Q5 *NUMBER *MIN 0 *MAX 50
Q5. How many units of {Q1} do you purchase per month?

*IF [Q5<1] *MSG "Please enter at least 1 unit"

*QUESTION Q6 *DATE
Q6. When did you last purchase {Q1}?

######################################################################
# TERMINATION POINTS
######################################################################

*QUESTION ScreenOut *TERMINATE
Thank you for your time.
Unfortunately, you do not qualify for this study.

######################################################################
# CLOSE
######################################################################

*QUESTION SurveyEnd *END
Thank you for completing the survey!
Your responses have been recorded.

######################################################################
# LANGUAGE SECTION — BENGALI
######################################################################

@LANGUAGE "Bengali"

*LIST "BrandList"
1:ব্র্যান্ড এ
2:ব্র্যান্ড বি
3:ব্র্যান্ড সি
4:ব্র্যান্ড ডি
99:উপরের কোনোটি নয়

*GRIDLIST "AgreementScale"
1:সম্পূর্ণ একমত
2:একমত
3:নিরপেক্ষ
4:অসম্মত
5:সম্পূর্ণ অসম্মত

*QUESTION FIFSInfo *FIFS
মাঠকর্মী ও সুপারভাইজারের তথ্য

*QUESTION S1 *SR
S1. আপনি কি আপনার পরিবারের প্রধান মুদি ক্রেতা?
1:হ্যাঁ
2:না

*QUESTION S2 *MR
S2. গত ৩ মাসে আপনি কোন ক্যাটাগরি ক্রয় করেছেন?
1:পানীয়
2:স্ন্যাকস
3:ব্যক্তিগত যত্ন
4:গৃহস্থালি
99:কোনোটিই নয়

*QUESTION Q1 *SR
প্রশ্ন ১. আপনি সবচেয়ে বেশি কোন পানীয় পান করেন?

*QUESTION Q3 *GRIDSR
প্রশ্ন ৩. নিচের বিবৃতিগুলোর সাথে আপনার একমত হওয়ার মাত্রা জানান।
1:এটি চমৎকার স্বাদের
2:এটি সাশ্রয়ী
3:এটি সহজলভ্য
4:আমি এটি সুপারিশ করব

*QUESTION SurveyEnd *END
সমীক্ষায় অংশগ্রহণের জন্য ধন্যবাদ!
```

---

## 12. Common Structural Mistakes

| Mistake | Error / Effect |
|---------|----------------|
| `*LIST` defined after the `*QUESTION` that uses it | Runtime error: list name not found |
| `*USELIST "name"` with wrong casing or typo | Error: list name not found |
| Header field missing (e.g. no `Script Name :`) | `Script Name Missing` in BuildResult.txt; output DB name is blank |
| `@LANGUAGE` without quotes around language name | `MessageBox: Invalid @LANGUAGE Syntax` |
| `*ENDREC` with trailing text | Error: not recognised as `*ENDREC` (must be exactly 7 chars) |
| `*REPEAT [name]` without `*ENDREPEAT` | Error: `*REPEAT block not closed with *ENDREPEAT` |
| Two questions with the same QId | `Duplicate QId` error |
| QId using a reserved word (e.g. `SELECT`) | `should not be used as QId` error |
| Attribute value used twice in same question | `Attribute value X is duplicate` error |
| Question text left empty | `Invalid Question Text : should not exist` |
| Unknown keyword on `*QUESTION` line | `Invalid Token : UNKNOWNKEYWORD` |
| No `*END` question | Survey has no defined completion point |
| No `*TERMINATE` question | Fine if no screen-out logic present |
| Missing mandatory QId (`FIFSInfo`, etc.) | `FIFSInfo question is missing..` in BuildResult.txt |
