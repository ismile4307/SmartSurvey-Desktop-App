# SmartSurvey Script — Attribute Modifiers Deep-Dive

> **Scope:** This document covers every modifier that can appear on an **attribute line**
> (`value:label *MODIFIER`) or on the **`*QUESTION` header line** (`*QUESTION QId *TYPE *MODIFIER`).
> Modifiers are split into two groups:
> - **Question-level modifiers** — on the `*QUESTION` header line, affect the whole question
> - **Attribute-level modifiers** — on individual `value:label` lines, affect that one option

---

## Part A — Question-Level Modifiers

These appear on the `*QUESTION QId *TYPE ...` header line.

---

### A1. Response Rotation & Randomisation

Controls the order in which attributes (options) are presented to the respondent.
All write to `T_Question.HasRandomAttrib`.

---

#### `*RANDOM`
**HasRandomAttrib = `"2"`**

Randomises the display order of all attributes on every interview, independently.

```
*QUESTION Q1 *MR *RANDOM
Which brands do you use?
1:Brand A
2:Brand B
3:Brand C
4:Brand D
```

**Behaviour:** Each respondent sees the options in a different random order.
`*NMUL` options are typically pinned to the bottom regardless of rotation.

---

#### `*ROT`
**HasRandomAttrib = `"1"`**

Rotates the attribute list in a fixed sequential pattern across interviews.
Interview 1 sees `A B C D`, Interview 2 sees `B C D A`, Interview 3 sees `C D A B`, etc.

```
*QUESTION Q2 *SR *ROT
Q2. Which brand comes to mind first?
1:Coca-Cola
2:Pepsi
3:Sprite
4:7UP
```

**Use case:** Eliminates position bias systematically while maintaining a structured rotation.
Preferred over `*RANDOM` when a uniform rotation pattern is required for weighting.

---

#### `*FROT` — Fixed Rotation
**HasRandomAttrib = `"5"`**

A fixed rotation variant — the starting position rotates but subsets within the list remain fixed.

```
*QUESTION Q3 *MR *FROT
Which features did you notice?
1:Price tag
2:Shelf position
3:Pack colour
4:Brand logo
```

**Difference from `*ROT`:** `*FROT` is used when groups of attributes should rotate as
fixed blocks rather than individual items.

---

#### `*OTPGROUPROT` — Option Rotation, Not Group
**HasRandomAttrib = `"10"`**

Rotates the individual **options** within each group, but keeps the **groups** themselves
in a fixed position.

```
*QUESTION Q4 *MR *OTPGROUPROT
...
```

**When to use:** When attributes are organised in labelled groups (`*GROUPNAME`)
and you want the items within each group to rotate but the group order to stay fixed.

---

#### `*OTPROTGROUP` — Group Rotation, Not Options
**HasRandomAttrib = `"01"`**

Rotates the **groups** themselves, but keeps the **options within each group** in fixed order.

```
*QUESTION Q5 *MR *OTPROTGROUP
...
```

**When to use:** Opposite of `*OTPGROUPROT` — group-level rotation with option-level stability.

---

#### `*OTPROTGROUPROT` — Both Option and Group Rotation
**HasRandomAttrib = `"11"`**

Rotates both the **groups** and the **options within groups**.

```
*QUESTION Q6 *MR *OTPROTGROUPROT
...
```

**When to use:** Full rotation at both levels — maximum position-bias elimination
when a complex grouped attribute list is presented.

---

#### Rotation Combinations Summary

| Modifier | HasRandomAttrib | Groups Rotate | Options Rotate |
|----------|----------------|---------------|----------------|
| `*RANDOM` | `"2"` | N/A | Yes (random) |
| `*ROT` | `"1"` | N/A | Yes (sequential) |
| `*FROT` | `"5"` | N/A | Yes (fixed blocks) |
| `*OTPGROUPROT` | `"10"` | No | Yes |
| `*OTPROTGROUP` | `"01"` | Yes | No |
| `*OTPROTGROUPROT` | `"11"` | Yes | Yes |

> **Rule:** Only one rotation modifier is permitted per question.
> Using two will trigger `Duplicate Token` error.

---

### A2. Question-Level Rotation

#### `*QROT` — Question Rotation
**HasRandomQntr = `"1"`**

Marks this question to participate in **question-order rotation** across the survey.
Questions with `*QROT` are rotated/randomised relative to other `*QROT` questions.

```
*QUESTION Q7 *SR *QROT
Please rate Brand A.
...

*QUESTION Q8 *SR *QROT
Please rate Brand B.
...
```

**Use case:** Rotating brand/stimulus evaluation blocks to eliminate order effects.

---

### A3. Group Rotation

#### `*GROUPROT n`
**HasMessageLogic = `n`**

Defines that this question uses grouped rotation with `n` groups.
Works in conjunction with `*GROUPNAME` on individual attributes.

```
*QUESTION Q9 *MR *GROUPROT 3
Which products do you use?
1:Shampoo *GROUPNAME "Hair Care"
2:Conditioner *GROUPNAME "Hair Care"
3:Face wash *GROUPNAME "Skin Care"
4:Moisturiser *GROUPNAME "Skin Care"
5:Body lotion *GROUPNAME "Skin Care"
6:Toothpaste *GROUPNAME "Oral Care"
7:Mouthwash *GROUPNAME "Oral Care"
```

**Rules:**
- `n` must be a positive integer
- `n` specifies how many groups rotate as blocks
- Combine with `*OTPGROUPROT`, `*OTPROTGROUP`, or `*OTPROTGROUPROT` to control
  what rotates within vs between groups

---

### A4. Response Count Constraints

#### `*MIN n`
**NoOfResponseMin = `n`**

Sets the **minimum number of responses** required for `*MR`, `*RANK`, `*NUMLIST`, etc.

```
*QUESTION Q10 *MR *MIN 2
Please select at least 2 brands you have tried.
1:Brand A
2:Brand B
3:Brand C
4:Brand D
```

**Also used for numeric range** (minimum acceptable value) on `*NUMBER` questions:
```
*QUESTION Q11 *NUMBER *MIN 18 *MAX 65
What is your age?
```

**Validation format:** Must be a non-negative integer (`^\d+$`) or decimal (`^\d.+$`).

---

#### `*MAX n`
**NoOfResponseMax = `n`**

Sets the **maximum number of responses** allowed. Also sets the required total
for `*NUMLISTTOTAL`, and the max value for `*NUMBER`.

```
*QUESTION Q12 *MR *MAX 3
Please select up to 3 brands.
...

*QUESTION Q13 *NUMLISTTOTAL *MAX 100
Allocate 100 points across the following categories.
...

*QUESTION Q14 *NUMBER *MIN 0 *MAX 999999
Enter monthly household income.
```

> **Note:** `*MAX` is detected with exclusion of `*MAXDIFF` to avoid conflict
> (`!myText.Contains("*MAXDIFF")`).

---

### A5. Layout & Display

#### `*COLUMN n`
**NumberOfColumn = `n`**

Renders attributes in `n` columns.

```
*QUESTION Q15 *SR *COLUMN 3
What is your marital status?
1:Single
2:Married
3:Separated
4:Divorced
5:Widowed
```

**Common values:** `1` (default, single column), `2`, `3`, `4`

---

#### `*HORIZONTAL`
**NumberOfColumn = `"2"`**

Shorthand for a 2-column horizontal layout. Equivalent to `*COLUMN 2`.

```
*QUESTION Q16 *SR *HORIZONTAL
Do you own a smartphone?
1:Yes
2:No
```

---

#### `*TAKEONLYONE`
**NumberOfColumn = `"4"`**

Shorthand for a 4-column layout, typically used for large option sets.

```
*QUESTION Q17 *SR *TAKEONLYONE
Select your age group.
1:Under 18
2:18–24
3:25–34
4:35–44
5:45–54
6:55+
```

---

#### `*SHOWASFORM`
**NumberOfColumn = `"3"`**

Renders the question using a form/table layout rather than a standard list.
Used on any question type (not just `*FORM`) to apply form-style display.

```
*QUESTION Q18 *SR *SHOWASFORM *USEGRIDLIST "YesNoList"
Please confirm the following details.
1:Name is correct
2:Address is correct
3:Contact number is correct
```

---

#### `*ADDSEARCH`
**NumberOfColumn = `"99"`**

Adds a search/filter input field above the attribute list.
Useful for long lists where the respondent needs to type to narrow down options.

```
*QUESTION Q19 *SR *ADDSEARCH
Select your city.
1:Dhaka
2:Chittagong
3:Sylhet
... (long list)
```

---

#### `*FONTSIZE n`
**WrittenOEInPaper = `n`**

Sets the text input font size in pixels for open-ended text fields.

```
*QUESTION Q20 *OPEN *FONTSIZE 18
Please describe your experience.
```

**Rules:** `n` must be a positive integer.

---

#### `*DIRIMAGE`
**WrittenOEInPaper = `"1"`**

Enables direct image display mode. Used to display images inline
without a separate image question.

```
*QUESTION Q21 *SR *DIRIMAGE *PICT "images/stimulus_a.jpg"
Which of the following packs is most attractive?
1:Pack A
2:Pack B
3:Pack C
```

---

#### `*SHOWASNUMTEXT`
**WrittenOEInPaper = `"1"`**

Renders a numeric field as a styled text box (same flag value as `*DIRIMAGE`).
Used when a numeric field should visually appear as a text input.

```
*QUESTION Q22 *NUMBER *SHOWASNUMTEXT
Enter your PIN code.
```

---

#### `*DELAY n`
**ShowInReport = `n`**

Auto-advances the screen after `n` milliseconds without requiring the respondent
to tap "Next". Primarily used for `*INFO`, `*IMAGE`, and `*MEDIA` questions.

```
*QUESTION Intro *INFO *DELAY 5000
Welcome to the survey. Please wait while we load the content.
```

**Rules:** `n` must be a positive integer (milliseconds).

---

### A6. Navigation Buttons

#### `*NOBACKBTN`
**DisplayBackButton = `"1"`**

Hides the "Back" navigation button on this question screen.

```
*QUESTION Q23 *SR *NOBACKBTN
Please confirm your final answer.
1:Yes, confirm
2:No, go back
```

> **Note:** The **first question** in the script automatically gets `DisplayBackButton = "1"`
> regardless of whether `*NOBACKBTN` is specified, since there is nowhere to go back to.

---

#### `*NONEXTBTN`
**DisplayNextButton = `"1"`**

Hides the "Next" navigation button. Combine with `*DELAY` to auto-advance,
or use when the question has its own built-in advance mechanism.

```
*QUESTION Q24 *INFO *NONEXTBTN *DELAY 3000
Please watch the video before continuing.
```

---

### A7. Address & Jump Buttons

These set `DisplayJumpButton` to control a special button shown alongside the question.

#### `*ADDRESS1` → DisplayJumpButton = `"1"`
#### `*ADDRESS2` → DisplayJumpButton = `"2"`
#### `*ADDRESS3` → DisplayJumpButton = `"3"`
#### `*ADDRESS4` → DisplayJumpButton = `"4"`

Used on address-related questions to activate a location-lookup or map-assist button
at the corresponding address level (street, city, district, country).

```
*QUESTION Q25 *OPEN *ADDRESS1
Enter street address.

*QUESTION Q26 *OPEN *ADDRESS2
Enter city/town.
```

---

#### `*TBC`
**DisplayJumpButton = `"TBC"`**

Marks a question as "To Be Confirmed" — displays a TBC indicator button.

```
*QUESTION Q27 *OPEN *TBC
Please confirm your email address.
```

---

### A8. Block & Jump Navigation

#### `*JUMPFOR n`
**ResumeQntrJump = `n`**

Skips forward `n` questions from the current position without evaluating them.
A hard-coded unconditional jump.

```
*QUESTION Q28 *INFO *JUMPFOR 3
This section is for respondents in Metro areas only.
```

The survey jumps forward 3 questions from Q28 (i.e., Q29, Q30, Q31 are skipped).

---

#### `*BLOCK n`
**ResumeQntrJump = `n`**

Similar to `*JUMPFOR` — stores the same `ResumeQntrJump` field.
Used to define a block of `n` questions that are treated as a visual unit.

```
*QUESTION Q29 *SR *BLOCK 5
Which section applies to you?
1:Section A
2:Section B
```

> **Note:** `*JUMPFOR` and `*BLOCK` both write to `ResumeQntrJump`.
> They should not be used together on the same question.

---

### A9. Image & Media

#### `*PICT "filename"`
**FilePath = `filename`**

Associates an image file with the question. The file path is relative to the
media folder configured on the device/server.

```
*QUESTION Q30 *IMAGE *PICT "images/product_coca_cola.jpg"
Please look at the image above.
```

**Format:** `*PICT "path/to/file.jpg"` — path must be enclosed in double quotes.
**Error:** `Syntax for *PICT is invalid` if format is wrong.

---

#### `*VIDEO "filename"`
**FilePath = `filename`**

Associates a video file with the question.

```
*QUESTION Q31 *MEDIA *VIDEO "videos/ad_clip_30sec.mp4"
Please watch the video, then answer the question below.
```

**Format:** Same as `*PICT`. Writes to the same `FilePath` field.

---

#### `*IMGADJBY n`
**HasMediaPath = `n`**

Adjusts the displayed image by `n` pixels (used to fine-tune image sizing/positioning).

```
*QUESTION Q32 *IMAGE *PICT "images/pack.jpg" *IMGADJBY 20
```

**Rules:** `n` must be a positive integer.

---

#### `*EXTCAMERA`
**Stored in:** `listOfKeyWords` only — signals device to use external/rear camera.

```
*QUESTION Q33 *CAPTUREIMAGE *EXTCAMERA
Please photograph the product shelf.
```

---

### A10. Question Label

#### `*QLABEL "text"`
**Comments = `text`**

Sets a short internal label/comment for the question. Used for tagging, reporting,
or identifying the question's purpose in the output database.

```
*QUESTION Q34 *SR *QLABEL "Brand Awareness TOM"
Which brand of soft drink comes to your mind first?
...
```

**Format:** Text must be enclosed in double quotes. No double quotes permitted inside the text.

---

### A11. Auto-Response (Dummy Patterns)

#### `*DUMMY1`
**HasAutoResponse = `"1"`**

Marks this question as a **pre-coded dummy** — its attributes are populated automatically
by `*INCLUDE`/`*EXCLUDE` logic rather than by the respondent.
The question is hidden from the respondent.

```
*QUESTION Q3Dummy *MR *DUMMY1
Brand Filter Dummy
1:Brand A
2:Brand B
3:Brand C
4:Brand D

*IF [Q1=1] *INCLUDE Q3Dummy [1;2]
*IF [Q1=2] *INCLUDE Q3Dummy [3;4]
```

**Pattern:** Define a dummy question with all possible values, then use `*IF ... *INCLUDE`
logic to populate it based on a previous answer. A visible question then uses
`*INCLUDE [DummyQId]` to inherit only the populated values.

```
*QUESTION Q4 *SR *INCLUDE [Q3Dummy]
Based on your previous answer, which brand do you prefer?
1:Brand A
2:Brand B
3:Brand C
4:Brand D
```

---

#### `*DUMMY2`
**HasAutoResponse = `"2"`**

Same concept as `*DUMMY1` but the second variant.
Used when the auto-response is system-driven (e.g., auto-coding from panel data,
language of interview, user ID assignment).

```
*QUESTION LanOfIntv *SR *DUMMY2
Language of Interview
1:English
2:Bengali
3:Hindi

*INCLUDE LanOfIntv LanguageOf[Interview]
```

**Common `*DUMMY2` use cases:**
- Auto-detecting language of interview
- Auto-assigning centre/market from interviewer ID
- Auto-coding zone or region from a previous selection
- Hidden routing variables that drive complex logic chains

---

### A12. Special Response Modifiers

#### `*DKCS "label" "code"`

Adds a **Don't Know / Can't Say** option block to the question.
Automatically creates **two hidden system attributes** on the question:

**Auto-generated attributes:**

| Position | AttributeEnglish | AttributeValue | IsExclusive | MinValue | Purpose |
|----------|-----------------|----------------|-------------|----------|---------|
| First | `""` (empty) | `"1"` | — | `"5"` | System lookup/control attribute |
| Second | `label` (from syntax) | `code` (from syntax) | `"1"` | — | The visible DKCS option |

```
*QUESTION Q10 *MR *DKCS "Don't Know" "99"
Which brands are you aware of?
1:Brand A
2:Brand B
3:Brand C
```

**Syntax rules:**
- Exactly two quoted strings: `*DKCS "label" "code"`
- Label must be non-empty
- Code must be numeric

**Behaviour:**
- The DKCS option (`IsExclusive = "1"`) is mutually exclusive — selecting it deselects all other options
- The first auto-attribute (`MinValue = "5"`) is a system marker for display control

**Error messages:**
```
Attribute code must be Number <code>
Attribute Label missing
Syntax for *DKCS is invalid
```

---

### A13. Force Open-Ended / Reload Flags

#### `*GROT`
**ForceToTakeOE = `"1"`**

Forces the question's open-ended follow-up to rotate through all selected responses
(group rotate OE). Each selected attribute triggers its own OE input.

```
*QUESTION Q11 *MR *GROT
Which brands did you notice? (Describe each one briefly)
1:Brand A
2:Brand B
3:Brand C
```

---

#### `*GRANDOM`
**ForceToTakeOE = `"2"`**

Randomises which selected attributes prompt an open-ended follow-up.

```
*QUESTION Q12 *MR *GRANDOM
Which brands did you notice?
...
```

---

#### `*INRLD`
**ForceToTakeOE = `"1"`** (same flag as `*GROT`)

Marks the question as "In Reload" — the question data should be reloaded/refreshed
from the server when this question is reached.

```
*QUESTION Q13 *SR *INRLD
Please select your current location.
...
```

---

## Part B — Attribute-Level Modifiers

These appear on individual `value:label` lines within a question block.

**Format:** `value:label text *MODIFIER1 *MODIFIER2 ...`

---

### B1. Response Type Modifiers

#### `*OPEN`
**TakeOpenended = `"1"`**

Adds a free-text input field for this specific option.
When a respondent selects this option, a text box appears for verbatim entry.

```
*QUESTION Q14 *MR
Which brands have you tried?
1:Brand A
2:Brand B
3:Brand C
95:Other *OPEN
```

**Restrictions:**
- Not permitted on `*FORM` attributes (`OPEN Should not be exist for Form attribute`)
- Can be combined with `*NMUL` but that combination is unusual

---

#### `*NMUL` — Not Multiple (Mutually Exclusive)
**IsExclusive = `"1"`**

Makes this attribute mutually exclusive — selecting it deselects all other
selected attributes, and selecting any other attribute deselects this one.

```
*QUESTION Q15 *MR
Which brands have you tried?
1:Brand A
2:Brand B
3:Brand C
4:Brand D
99:None of the above *NMUL
```

**Common use:** "None of the above", "Don't Know", "Not applicable" options.

**Restrictions:**
- Not permitted on `*FORM` attributes (`NMUL Should not be exist for Form attribute`)

---

#### `*NOCON` — No Consolidation
**IsExclusive = `"2"`**

Marks this attribute as excluded from consolidation in reporting/data processing.
Also adds the attribute's value to the `ExceptionalValue` tracking list.

```
*QUESTION Q16 *MR
Which of the following apply to you?
1:I buy weekly
2:I buy monthly
3:I rarely buy
97:Not sure *NOCON
98:Refused *NOCON
99:None *NMUL
```

**Notes:**
- `IsExclusive = "2"` is distinct from `*NMUL` (`IsExclusive = "1"`)
- `*NOCON` does not make the option mutually exclusive — it only flags it for reporting exclusion
- The attribute value is added to `myAttributeFilter.ExceptionalValue` as a comma-separated list
- **Restrictions:** Not permitted on `*FORM` attributes

---

### B2. Mandatory & Validation

#### `*MANDATORY`
**ForceAndMsgOpt = `"11"`**

Forces this specific attribute to be filled in.
Used on attributes within `*ALPHALIST`, `*NUMLIST`, `*NUMLISTTOTAL`, and `*FORM` questions
to require entry for that particular row.

```
*QUESTION Q17 *ALPHALIST
Please enter the names of brands you use.
1:First brand *MANDATORY
2:Second brand
3:Third brand
```

---

#### `*MIN n`
**MinValue = `n`**

Sets the minimum acceptable numeric value for this attribute's input.
Used on individual rows of `*NUMLIST`, `*NUMLISTTOTAL`, `*GRIDNUM`, and `*FORM` questions.

```
*QUESTION Q18 *NUMLIST
How many units do you buy per month?
1:Shampoo *MIN 0 *MAX 50
2:Conditioner *MIN 0 *MAX 30
3:Body wash *MIN 0
```

**Rules:** `n` must be a non-negative integer or decimal (`^\d+$` or `^\d.+$`).

---

#### `*MAX n`
**MaxValue = `n`**

Sets the maximum acceptable numeric value for this attribute's input.

```
1:Grocery spend *MIN 0 *MAX 10000
```

**Rules:** Same as `*MIN`.

---

### B3. Media on Attributes

#### `*PICT "path"`

Associates an image with this specific option.

**Behaviour varies by question type:**
- For `*ALPHALIST` (QType 12), `*NUMLIST` (QType 13), `*NUMLISTTOTAL` (QType 17):
  → path stored in `AttributeMain.Comments`
- For all other types:
  → path stored in `AttributeMain.ForceAndMsgOpt`

```
*QUESTION Q19 *SR
Which pack design is most attractive?
1:Classic design *PICT "images/pack_classic.jpg"
2:Modern design *PICT "images/pack_modern.jpg"
3:Premium design *PICT "images/pack_premium.jpg"
```

**Format:** `*PICT "path/to/image.jpg"` — double quotes required.

---

#### `*VIDEO "path"`

Associates a video clip with this specific option.
Path stored in `AttributeMain.ForceAndMsgOpt`.

```
*QUESTION Q20 *SR
Which advertisement did you find most memorable?
1:Ad Version A *VIDEO "videos/ad_a.mp4"
2:Ad Version B *VIDEO "videos/ad_b.mp4"
```

---

### B4. Grouping Modifiers

#### `*GROUPNAME "label"`
**GroupName = `label`**

Assigns this attribute to a named group, creating a visual section header in the list.
Used with group rotation modifiers (`*GROUPROT`, `*OTPGROUPROT`, etc.) on the question.

```
*QUESTION Q21 *MR *GROUPROT 3
Which products do you currently use?
1:Shampoo *GROUPNAME "Hair Care"
2:Conditioner *GROUPNAME "Hair Care"
3:Hair oil *GROUPNAME "Hair Care"
4:Face wash *GROUPNAME "Skin Care"
5:Moisturiser *GROUPNAME "Skin Care"
6:Sunscreen *GROUPNAME "Skin Care"
7:Toothpaste *GROUPNAME "Oral Care"
8:Mouthwash *GROUPNAME "Oral Care"
```

**Rules:**
- Group name in double quotes
- Multiple attributes can share the same group name
- Groups are displayed as collapsible sections or labelled blocks

---

#### `*GROUPHEAD`
**Comments = `"GroupHead"`**

Marks this attribute as a **group header row** — it acts as a section label
but is not itself a selectable option.

```
*QUESTION Q22 *MR
Select all that apply.
1:Hair care products *GROUPHEAD
2:Shampoo
3:Conditioner
4:Skin care products *GROUPHEAD
5:Face wash
6:Moisturiser
```

**Notes:**
- Sets `attributeMain.Comments = "GroupHead"` in the database
- The `*GROUPHEAD` attribute typically has an unusable value (e.g., `0` or a separator value)
- Can be combined with `*NMUL` if the header itself should act as "select all in group"

---

### B5. Attribute-Level Filtering

These modifiers on an attribute line apply a row-level conditional filter,
independent of the question-level `*INCLUDE [QId]` filter.

#### `*INCLUDE [QId]` on an attribute
**FilterQid = `QId`, FilterType = `"1"`**

Shows this specific attribute only if the respondent answered `QId`.

```
*QUESTION Q23 *GRIDSR *USEGRIDLIST "Scale5"
Rate each brand you are aware of.
1:Brand A *INCLUDE [Q1Aware]
2:Brand B *INCLUDE [Q1Aware]
3:Brand C *INCLUDE [Q1Aware]
```

**For GRIDMR / GRIDNUM questions:** The filter is stored as
`FilterQid = QId_AttributeValue` (e.g., `Q1Aware_1`) to match the specific grid row.

---

#### `*EXCLUDE [QId]` on an attribute
**FilterQid = `QId`, FilterType = `"2"`**

Hides this specific attribute if the respondent answered `QId`.

```
1:Brand X *EXCLUDE [Q1TOM]
2:Brand Y *EXCLUDE [Q1TOM]
```

---

### B6. Special Purpose Modifiers

#### `*LAT` (GPS attribute)
**MinValue = `n`** (latitude precision value)

Marks this attribute row as the **latitude capture field** for a `*GPS` question.

```
*QUESTION Q24 *GPS
Please allow location access.
1:Latitude *LAT
2:Longitude *LON
```

---

#### `*LON` (GPS attribute)
**MaxValue = `n`** (longitude precision value)

Marks this attribute row as the **longitude capture field**.

---

#### `*COMPVAL n`
**ExcepValue = `n`**

Sets a comparison value on this attribute. Used for internal comparisons
in logic or data processing.

```
1:Target area *COMPVAL 500
```

**Rules:** `n` must be numeric.

---

#### `*EXCEPT [value]`
**ExcepValue = `value`**

Sets an exception/exclusion value for this attribute in filter logic.
When this attribute's `ExcepValue` is set, the system treats `value` as a
special non-standard response code.

```
1:Category A *EXCEPT [97]
```

**Rules:** `value` inside brackets must be numeric.

---

### B7. Form Sub-Type Modifiers

Used **only** on attributes within a `*FORM` question.
Each attribute in a form must specify its own input type.

#### `*SR` → LinkId1 = `"1"` (Single Response sub-field)
#### `*MR` → LinkId1 = `"2"` (Multiple Response sub-field)
#### `*ALPHA` → LinkId1 = `"3"` (Text entry sub-field)
#### `*NUMBER` → LinkId1 = `"4"` (Numeric entry sub-field)
#### `*DATE` → LinkId1 = `"14"` (Date picker sub-field)
#### `*TIME` → LinkId1 = `"15"` (Time picker sub-field)
#### `*AUTOCOMPLETE` → LinkId1 = `"22"` (Autocomplete sub-field)
#### `*DROPDOWN` → LinkId1 = `"24"` (Dropdown sub-field)

```
*QUESTION Q25 *FORM
Please fill in the following details.
1:Full Name *ALPHA *MANDATORY
2:Age *NUMBER *MIN 18 *MAX 99 *MANDATORY
3:Gender *SR *USEGRIDLIST "GenderList" *MANDATORY
4:City *DROPDOWN *USEGRIDLIST "CityList"
5:Date of Visit *DATE *MANDATORY
```

**Validation rules for `*FORM` attributes:**
- `*SR`, `*MR`, `*DROPDOWN`, `*AUTOCOMPLETE` → must also have `*USEGRIDLIST "name"`
- `*OPEN` and `*NMUL` are **not permitted** on form attributes
- `*NOCON` is **not permitted** on form attributes
- An attribute with no sub-type modifier triggers: `Form attribute must have a type`

---

#### `*USEGRIDLIST "name"` on an attribute (PSCALE / FORM)
**LinkId2 = `name`**

Links this specific attribute row to its own grid list.
For `*PSCALE` each row must specify its own `*USEGRIDLIST`.
For `*FORM` each `*SR`, `*MR`, `*DROPDOWN`, `*AUTOCOMPLETE` row must specify one.

```
*QUESTION Q26 *PSCALE
Please rate the brand on each dimension.
1:Value *SR *USEGRIDLIST "ValueScale"
2:Quality *SR *USEGRIDLIST "QualityScale"
3:Trust *SR *USEGRIDLIST "TrustScale"
```

**Validation for `*PSCALE`:** All grid lists referenced must have the **same number of entries**.
If they differ: `PSCALE grid list "X" has N attributes but expected M (same as "Y")`.

---

## Part C — Modifier Combinations Quick Reference

### Valid on `*SR` questions

| Modifier | Allowed | Notes |
|----------|---------|-------|
| `*ROT` / `*RANDOM` / `*FROT` | Yes | Rotate options |
| `*QROT` | Yes | Question-level rotation |
| `*GROUPROT n` | Yes | Group rotation |
| `*COLUMN n` | Yes | Layout |
| `*HORIZONTAL` | Yes | = `*COLUMN 2` |
| `*DKCS "label" "code"` | Yes | Adds DK/CS option |
| `*INCLUDE [QId]` | Yes | Attribute filter |
| `*EXCLUDE [QId]` | Yes | Attribute filter |
| `*INCLUDEGRIDLIST [QId]` | No | Grid-specific |
| `*DUMMY1` / `*DUMMY2` | Yes | Auto-fill mode |
| `*MIN` / `*MAX` | No (at question level) | Use on attributes |
| `*IF [condition]` | Yes | Conditional display |

### Valid on `*MR` questions

All `*SR` modifiers plus:

| Modifier | Allowed | Notes |
|----------|---------|-------|
| `*MIN n` | Yes | Minimum selections |
| `*MAX n` | Yes | Maximum selections |
| `*GROT` / `*GRANDOM` | Yes | Force OE rotation |
| `*OTPGROUPROT` etc. | Yes | Group-option rotation |

### Valid on `*GRIDSR` / `*GRIDMR` questions

| Modifier | Required | Notes |
|----------|---------|-------|
| `*USEGRIDLIST "name"` | Yes | Column definitions |
| `*INCLUDEGRIDLIST [QId]` | No | Filter rows |
| `*ROT` / `*RANDOM` | No | Rotate rows |
| `*QROT` | No | Question rotation |
| `*MIN` / `*MAX` | No | On grid header |

### Valid on `*FORM` attributes only

`*SR`, `*MR`, `*ALPHA`, `*NUMBER`, `*DATE`, `*TIME`, `*AUTOCOMPLETE`, `*DROPDOWN`,
`*USEGRIDLIST "name"`, `*MIN n`, `*MAX n`, `*MANDATORY`

**Forbidden on `*FORM` attributes:** `*OPEN`, `*NMUL`, `*NOCON`

---

## Part D — DB Field Mapping Summary

| Modifier | DB Table | DB Field | Value |
|----------|----------|----------|-------|
| `*RANDOM` | T_Question | HasRandomAttrib | `"2"` |
| `*ROT` | T_Question | HasRandomAttrib | `"1"` |
| `*FROT` | T_Question | HasRandomAttrib | `"5"` |
| `*OTPGROUPROT` | T_Question | HasRandomAttrib | `"10"` |
| `*OTPROTGROUP` | T_Question | HasRandomAttrib | `"01"` |
| `*OTPROTGROUPROT` | T_Question | HasRandomAttrib | `"11"` |
| `*QROT` | T_Question | HasRandomQntr | `"1"` |
| `*GROUPROT n` | T_Question | HasMessageLogic | n |
| `*MIN n` (question) | T_Question | NoOfResponseMin | n |
| `*MAX n` (question) | T_Question | NoOfResponseMax | n |
| `*DUMMY1` | T_Question | HasAutoResponse | `"1"` |
| `*DUMMY2` | T_Question | HasAutoResponse | `"2"` |
| `*COLUMN n` | T_Question | NumberOfColumn | n |
| `*HORIZONTAL` | T_Question | NumberOfColumn | `"2"` |
| `*TAKEONLYONE` | T_Question | NumberOfColumn | `"4"` |
| `*SHOWASFORM` | T_Question | NumberOfColumn | `"3"` |
| `*ADDSEARCH` | T_Question | NumberOfColumn | `"99"` |
| `*FONTSIZE n` | T_Question | WrittenOEInPaper | n |
| `*DIRIMAGE` | T_Question | WrittenOEInPaper | `"1"` |
| `*SHOWASNUMTEXT` | T_Question | WrittenOEInPaper | `"1"` |
| `*DELAY n` | T_Question | ShowInReport | n |
| `*NOBACKBTN` | T_Question | DisplayBackButton | `"1"` |
| `*NONEXTBTN` | T_Question | DisplayNextButton | `"1"` |
| `*ADDRESS1-4` | T_Question | DisplayJumpButton | `"1"`–`"4"` |
| `*TBC` | T_Question | DisplayJumpButton | `"TBC"` |
| `*JUMPFOR n` | T_Question | ResumeQntrJump | n |
| `*BLOCK n` | T_Question | ResumeQntrJump | n |
| `*GROT` | T_Question | ForceToTakeOE | `"1"` |
| `*GRANDOM` | T_Question | ForceToTakeOE | `"2"` |
| `*INRLD` | T_Question | ForceToTakeOE | `"1"` |
| `*IMGADJBY n` | T_Question | HasMediaPath | n |
| `*PICT "p"` (question) | T_Question | FilePath | p |
| `*VIDEO "p"` (question) | T_Question | FilePath | p |
| `*QLABEL "t"` | T_Question | Comments | t |
| `*DKCS "l" "c"` | T_OptAttribute | Auto-creates 2 rows | see §A12 |
| `*OPEN` (attribute) | T_OptAttribute | TakeOpenended | `"1"` |
| `*NMUL` (attribute) | T_OptAttribute | IsExclusive | `"1"` |
| `*NOCON` (attribute) | T_OptAttribute | IsExclusive | `"2"` |
| `*MANDATORY` (attribute) | T_OptAttribute | ForceAndMsgOpt | `"11"` |
| `*MIN n` (attribute) | T_OptAttribute | MinValue | n |
| `*MAX n` (attribute) | T_OptAttribute | MaxValue | n |
| `*PICT "p"` (attr, types 12/13/17) | T_OptAttribute | Comments | p |
| `*PICT "p"` (attr, other types) | T_OptAttribute | ForceAndMsgOpt | p |
| `*VIDEO "p"` (attribute) | T_OptAttribute | ForceAndMsgOpt | p |
| `*GROUPNAME "n"` | T_OptAttribute | GroupName | n |
| `*GROUPHEAD` | T_OptAttribute | Comments | `"GroupHead"` |
| `*INCLUDE [QId]` (attr) | T_OptAttribute | FilterQid / FilterType | QId / `"1"` |
| `*EXCLUDE [QId]` (attr) | T_OptAttribute | FilterQid / FilterType | QId / `"2"` |
| `*EXCEPT [v]` | T_OptAttribute | ExcepValue | v |
| `*COMPVAL n` | T_OptAttribute | ExcepValue | n |
| `*LAT` | T_OptAttribute | MinValue | (value) |
| `*LON` | T_OptAttribute | MaxValue | (value) |
| `*SR` / `*MR` / `*ALPHA` etc. (form attr) | T_OptAttribute | LinkId1 | see §B7 |
| `*USEGRIDLIST "n"` (attr) | T_OptAttribute | LinkId2 | n |
