# SmartSurvey Script — Question Types Reference

> **Scope:** This document covers every question type supported by the SmartSurvey script compiler (`FrmBuildScript.xaml.cs`).
> Each question begins with `*QUESTION <QId>` on the header line, followed by the type keyword and any modifiers.
> The question text occupies the next line(s), then attributes (options) follow.

---

## General Question Syntax

```
*QUESTION <QId> *<TYPE> [*MODIFIER1] [*MODIFIER2] ...
Question text here
[attribute lines / *USELIST / *USEGRIDLIST]
```

**QId Rules:**
- Alphanumeric only (`^[a-zA-Z0-9]+$`)
- Must start with a letter
- Must be unique across the entire script
- Cannot use reserved words: `UNION`, `ABS`, `JOIN`, `SELECT`, `INTO`, `WHERE`, `IF`, `EXISTS`, `ORDER`, `BY`, `UPDATE`, `DELETE`, `MAX`, `MIN`

---

## Complete Type Reference

---

### Type 1 — Single Response (`*SR`)

**QType ID:** `1`
**Keyword:** `*SR`

Presents a list of options where the respondent can select **exactly one** answer.

**Syntax:**
```
*QUESTION Q1 *SR
What is your gender?
1:Male
2:Female
3:Other
```

**Common Modifiers:** `*ROT`, `*RANDOM`, `*FROT`, `*COLUMN`, `*HORIZONTAL`, `*MIN`, `*MAX`, `*DKCS`, `*MANDATORY`

**Notes:**
- Attributes listed as `value:label`
- Use `*DKCS` to add a "Don't Know / Can't Say" exclusive option
- Use `*NMUL` on any attribute to make it mutually exclusive
- Use `*OPEN` on any attribute to allow a free-text entry for that option

---

### Type 2 — Multiple Response (`*MR`)

**QType ID:** `2`
**Keyword:** `*MR`

Presents a list of options where the respondent can select **one or more** answers.

**Syntax:**
```
*QUESTION Q2 *MR *MAX 3
Which of the following brands do you use?
1:Brand A
2:Brand B
3:Brand C
4:Brand D *NMUL
```

**Common Modifiers:** `*ROT`, `*RANDOM`, `*MIN`, `*MAX`, `*COLUMN`, `*HORIZONTAL`, `*DKCS`, `*GROUPROT`, `*GROUPNAME`

**Notes:**
- `*MAX n` limits maximum number of selections
- `*MIN n` enforces minimum number of selections
- Adding `*NMUL` to an attribute makes it mutually exclusive (e.g., "None of the above")
- Adding `*NOCON` to an attribute prevents it from being consolidated in reporting

---

### Type 3 — Open Ended (`*OPEN`)

**QType ID:** `3`
**Keyword:** `*OPEN`

Presents a free-text input field for verbatim responses.

**Syntax:**
```
*QUESTION Q3 *OPEN
Please describe your experience.
```

**Common Modifiers:** `*MANDATORY`, `*FONTSIZE`, `*DELAY`

**Notes:**
- No attributes required
- `*FONTSIZE n` sets the text input font size in pixels
- Cannot be combined with `*ALPHALIST` (which is a separate type)
- Use `{QId}` in the question text to pipe in a previous answer

---

### Type 4 — Numeric (`*NUMBER`)

**QType ID:** `4`
**Keyword:** `*NUMBER`

Displays a numeric keypad / input field for a single numeric value.

**Syntax:**
```
*QUESTION Q4 *NUMBER *MIN 0 *MAX 120
How old are you?
```

**Common Modifiers:** `*MIN`, `*MAX`, `*MANDATORY`, `*SHOWASNUMTEXT`

**Notes:**
- `*MIN` and `*MAX` define the allowed numeric range
- `*SHOWASNUMTEXT` renders the field as a text box styled for numbers
- Cannot be combined with `*NUMBEROFRESPONSE` (different directive)

---

### Type 5 — Ranking (`*RANK`)

**QType ID:** `5`
**Keyword:** `*RANK`

Asks respondents to rank a list of items in order of preference.

**Syntax:**
```
*QUESTION Q5 *RANK *MAX 3
Please rank the following from most to least preferred.
1:Price
2:Quality
3:Brand
4:Service
```

**Common Modifiers:** `*MAX`, `*ROT`, `*RANDOM`, `*COLUMN`

**Notes:**
- `*MAX n` defines how many items must be ranked
- Items are drag-and-drop or numbered entry on device

---

### Type 6 — Image Display (`*IMAGE`)

**QType ID:** `6`
**Keyword:** `*IMAGE`

Displays an image on screen. Used for showcasing stimuli (product images, ads, etc.).

**Syntax:**
```
*QUESTION Q6 *IMAGE *PICT "images/product_a.jpg"
Please look at the image shown above.
```

**Common Modifiers:** `*PICT "path"`, `*VIDEO "path"`, `*IMGADJBY`, `*IMGSIZE`, `*DELAY`, `*DIRIMAGE`

**Notes:**
- `*PICT "filename"` specifies the image file path
- `*VIDEO "filename"` specifies a video file path
- `*IMGADJBY n` adjusts the image by n pixels
- `*DELAY n` auto-advances after n milliseconds
- `*DIRIMAGE` sets the direct image display flag
- Keyword detection excludes `*CAPTUREIMAGE` (separate type)

---

### Type 7 — Grid Single Response (`*GRIDSR`)

**QType ID:** `7`
**Keyword:** `*GRIDSR`

A grid where each row is a question/item and the respondent selects **one** column option per row.

**Syntax:**
```
*GRIDLIST "ScaleList"
1:Strongly Agree
2:Agree
3:Neutral
4:Disagree
5:Strongly Disagree

*QUESTION Q7 *GRIDSR *USEGRIDLIST "ScaleList"
Please rate the following statements.
*USELIST "StatementsList"
```

**Common Modifiers:** `*USEGRIDLIST "name"`, `*USELIST "name"`, `*INCLUDEGRIDLIST [QId]`, `*ROT`, `*RANDOM`, `*QROT`, `*MANDATORY`

**Notes:**
- Requires both a `*GRIDLIST` (columns) and a `*LIST` or inline attributes (rows)
- `*INCLUDEGRIDLIST [QId]` filters rows based on answers to a previous question
- Each row attribute can carry `*OPEN`, `*NMUL`, min/max constraints

---

### Type 8 — Grid Multiple Response (`*GRIDMR`)

**QType ID:** `8`
**Keyword:** `*GRIDMR`

A grid where each row allows **multiple** column selections.

**Syntax:**
```
*QUESTION Q8 *GRIDMR *USEGRIDLIST "BrandList"
Which brands do you associate with each attribute?
1:Value for money
2:Premium quality
3:Wide availability
```

**Common Modifiers:** `*USEGRIDLIST "name"`, `*ROT`, `*RANDOM`, `*QROT`, `*MIN`, `*MAX`

**Notes:**
- Same structure as `*GRIDSR` but allows multiple column selections per row
- Sets `qTypeForGridQid = 8` internally for proper QId tracking

---

### Type 9 — Media (`*MEDIA`)

**QType ID:** `9`
**Keyword:** `*MEDIA`

Plays audio or video content before or during the question. Used for stimulus exposure.

**Syntax:**
```
*QUESTION Q9 *MEDIA *PICT "audio/ad_clip.mp3"
Please listen to the audio clip, then answer.
```

**Common Modifiers:** `*PICT "path"`, `*VIDEO "path"`, `*DELAY`, `*NONEXTBTN`

**Notes:**
- `*DELAY n` can be used to hold the screen for n milliseconds
- Typically followed by rating or recall questions

---

### Type 10 — Recording (`*RECORDING`)

**QType ID:** `10`
**Keyword:** `*RECORDING`

Captures an audio recording from the respondent's device microphone.

**Syntax:**
```
*QUESTION Q10 *RECORDING
Please record your feedback in your own words.
```

**Notes:**
- No attributes needed
- Silent background recording uses `*STARTREC` / `*ENDREC` block directives instead
- Recording data is stored against the QId in the response database

---

### Type 12 — Alpha List (`*ALPHALIST`)

**QType ID:** `12`
**Keyword:** `*ALPHALIST`

Displays a list where each row accepts a **text (alphabetic) input** from the respondent.

**Syntax:**
```
*QUESTION Q12 *ALPHALIST *MAX 3
Please enter up to 3 brand names you recall.
1:Brand 1
2:Brand 2
3:Brand 3
```

**Common Modifiers:** `*MIN`, `*MAX`, `*MANDATORY`, `*PICT`

**Notes:**
- Each row is a separate text entry field
- `LinkId1 = "3"` (ALPHA) is applied to each attribute internally
- `*PICT "path"` on an attribute stores the image path in `Comments` field

---

### Type 13 — Numeric List (`*NUMLIST`)

**QType ID:** `13`
**Keyword:** `*NUMLIST`

Displays a list where each row accepts a **numeric input**.

**Syntax:**
```
*QUESTION Q13 *NUMLIST
Please enter the quantity purchased for each category.
1:Grocery
2:Electronics
3:Clothing
```

**Common Modifiers:** `*MIN`, `*MAX`, `*MANDATORY`

**Notes:**
- Each row is a separate numeric entry
- `LinkId1 = "4"` (NUMBER) is applied internally
- Cannot be combined with `*NUMLISTTOTAL` (separate type)
- `*PICT "path"` supported on individual attributes

---

### Type 14 — Date (`*DATE`)

**QType ID:** `14`
**Keyword:** `*DATE`

Presents a date picker for capturing a specific date.

**Syntax:**
```
*QUESTION Q14 *DATE
What is your date of birth?
```

**Common Modifiers:** `*MANDATORY`

**Notes:**
- No attributes needed
- `LinkId1 = "14"` used when referenced in grid types
- Date value stored in standard date format

---

### Type 15 — Time (`*TIME`)

**QType ID:** `15`
**Keyword:** `*TIME`

Presents a time picker for capturing a specific time.

**Syntax:**
```
*QUESTION Q15 *TIME
What time did you arrive at the store?
```

**Common Modifiers:** `*MANDATORY`

**Notes:**
- No attributes needed
- `LinkId1 = "15"` used when referenced in grid types
- Can be used with `TIMEDIFFOF[Q15, Q16]` in logic conditions

---

### Type 16 — Capture Image (`*CAPTUREIMAGE`)

**QType ID:** `16`
**Keyword:** `*CAPTUREIMAGE`

Opens the device camera and captures a photo as part of the survey response.

**Syntax:**
```
*QUESTION Q16 *CAPTUREIMAGE
Please take a photo of the product shelf.
```

**Common Modifiers:** `*EXTCAMERA`, `*MANDATORY`

**Notes:**
- `*EXTCAMERA` switches to the external/rear camera
- Captured image is stored and linked to the response record
- Distinct from `*IMAGE` (which only displays an image)

---

### Type 17 — Numeric List Total (`*NUMLISTTOTAL`)

**QType ID:** `17`
**Keyword:** `*NUMLISTTOTAL`

A numeric list where the values must sum to a specified total.

**Syntax:**
```
*QUESTION Q17 *NUMLISTTOTAL *MAX 100
Please allocate 100 points across the following.
1:Price
2:Quality
3:Service
4:Design
```

**Common Modifiers:** `*MAX` (sets the required total), `*MIN`, `*MANDATORY`

**Notes:**
- `*MAX` defines the target total (e.g., 100 for percentage allocation)
- Validation enforces that entries sum exactly to the target
- Each row gets `LinkId1 = "4"` (NUMBER) internally

---

### Type 22 — Autocomplete (`*AUTOCOMPLETE` / `*AUTOCOMPLETELIST`)

**QType ID:** `22`
**Keywords:**
- `*AUTOCOMPLETE` — single autocomplete field (no pre-defined list)
- `*AUTOCOMPLETELIST` — autocomplete from a defined list of options

A searchable text field that suggests matches as the respondent types.

**Syntax (single field):**
```
*QUESTION Q22a *AUTOCOMPLETE *USEGRIDLIST "CityList"
Start typing your city name.
```

**Syntax (with list):**
```
*QUESTION Q22b *AUTOCOMPLETELIST
Which occupation applies to you?
1:Doctor
2:Engineer
3:Teacher
4:Business Owner
```

**Common Modifiers:** `*USEGRIDLIST "name"`, `*MANDATORY`, `*ADDSEARCH`

**Notes:**
- `*AUTOCOMPLETE` without `*AUTOCOMPLETELIST` sets `hasSingleDropdown = true`, auto-creating a single attribute with `LinkId1 = "1"`, `ForceAndMsgOpt = "11"`
- `*AUTOCOMPLETELIST` uses inline attributes as the suggestion source
- `*ADDSEARCH` sets `NumberOfColumn = "99"` to enable search mode
- `LinkId1 = "22"` is used when this type is referenced in a grid

---

### Type 23 — Autocomplete Answer (`*AUTOCOMPLETEANS`)

**QType ID:** `23`
**Keyword:** `*AUTOCOMPLETEANS`

A variant of autocomplete that captures the answer from a previous autocomplete question's response list.

**Syntax:**
```
*QUESTION Q23 *AUTOCOMPLETEANS
Select your answer from the suggested list.
```

**Notes:**
- Used in chained autocomplete scenarios
- Typically paired with a preceding `*AUTOCOMPLETE` question

---

### Type 24 — Dropdown (`*DROPDOWN` / `*DROPDOWNLIST`)

**QType ID:** `24`
**Keywords:**
- `*DROPDOWN` — single dropdown (no pre-defined list, linked via `*USEGRIDLIST`)
- `*DROPDOWNLIST` — dropdown with inline options

A dropdown/picker control for selecting one option from a list.

**Syntax (with grid list):**
```
*QUESTION Q24a *DROPDOWN *USEGRIDLIST "RegionList"
Please select your region.
```

**Syntax (inline list):**
```
*QUESTION Q24b *DROPDOWNLIST
What is your highest education level?
1:Primary
2:Secondary
3:Graduate
4:Post-Graduate
```

**Common Modifiers:** `*USEGRIDLIST "name"`, `*MANDATORY`, `*ADDSEARCH`

**Notes:**
- `*DROPDOWN` without `*DROPDOWNLIST` sets `hasSingleDropdown = true`, auto-creating a mandatory single attribute
- `*ADDSEARCH` adds a search/filter field inside the dropdown
- `LinkId1 = "24"` when referenced from a grid
- `NumberOfColumn = "3"` if `*SHOWASFORM` is also applied

---

### Type 26 — Drag Drop (`*DRAGDROP`)

**QType ID:** `26`
**Keyword:** `*DRAGDROP`

Respondents drag items from one area to another to assign them to categories.

**Syntax:**
```
*QUESTION Q26 *DRAGDROP *USEGRIDLIST "CategoryList"
Drag each brand into the category that best fits.
*USELIST "BrandList"
```

**Common Modifiers:** `*USEGRIDLIST "name"`, `*USELIST "name"`, `*ROT`, `*RANDOM`

**Notes:**
- Requires a `*GRIDLIST` (destination categories) and a `*LIST` or inline attributes (items to drag)
- Functions similarly to `*GRIDSR` in data storage

---

### Type 27 — Grid Numeric (`*GRIDNUM`)

**QType ID:** `27`
**Keyword:** `*GRIDNUM`

A grid where each cell accepts a numeric value.

**Syntax:**
```
*QUESTION Q27 *GRIDNUM *USEGRIDLIST "WeekList"
Please enter your spend amount for each category per week.
1:Food
2:Transport
3:Entertainment
```

**Common Modifiers:** `*USEGRIDLIST "name"`, `*MIN`, `*MAX`, `*MANDATORY`, `*INCLUDEGRIDLIST [QId]`

**Notes:**
- Each cell is a numeric input field
- `*MIN` / `*MAX` apply per cell
- `LinkId1 = "27"` used when referenced in logic conditions
- Sets `qTypeForGridQid = 8` internally

---

### Type 32 — Paired Scale (`*PSCALE`)

**QType ID:** `32`
**Keyword:** `*PSCALE`

A paired comparison scale (semantic differential) where rows are rated between two opposing anchors defined by the grid columns.

**Syntax:**
```
*GRIDLIST "ScalePairs"
1:Affordable
2:Premium

*QUESTION Q32 *PSCALE *USEGRIDLIST "ScalePairs"
Please rate the brand on the following dimensions.
1:Value
2:Prestige
3:Innovation
```

**Common Modifiers:** `*USEGRIDLIST "name"`, `*USELIST "name"`, `*ROT`, `*MANDATORY`

**Notes:**
- Validation enforces that **all referenced grid lists must have the same number of entries**
- Columns are displayed as bipolar anchors (left vs. right)

---

### Type 40 — MaxDiff (`*MAXDIFF`)

**QType ID:** `40`
**Keyword:** `*MAXDIFF`

Maximum Difference (Best-Worst) scaling. Respondents choose the best and worst item from a set.

**Syntax:**
```
*QUESTION Q40 *MAXDIFF
Which of these features is most and least important to you?
1:Battery Life
2:Camera Quality
3:Screen Size
4:Price
5:Brand
```

**Common Modifiers:** `*ROT`, `*RANDOM`, `*MAX`

**Notes:**
- Usually scripted with a defined set size (e.g., 4 items shown per screen)
- Used for implicit importance measurement

---

### Type 41 — GPS (`*GPS`)

**QType ID:** `41`
**Keyword:** `*GPS`

Captures the device's geographic location (latitude and longitude).

**Syntax:**
```
*QUESTION Q41 *GPS
Please allow location access to proceed.
```

**Attribute Modifiers (per attribute line):**
- `*LAT` — marks this attribute as the latitude capture
- `*LON` — marks this attribute as the longitude capture

**Notes:**
- Typically requires no visible attributes
- `*LAT` and `*LON` modifiers on inline attributes map to `T_OptAttribute.Comments`
- Location data used in `DISTANCEFROM[Q41]` and `DISTANCEBTNOF[Q41,Q42]` logic functions

---

### Type 48 — Form (`*FORM`)

**QType ID:** `48`
**Keyword:** `*FORM`

A form-style layout where multiple input fields are displayed in a table/form structure on a single screen.

**Syntax:**
```
*QUESTION Q48 *FORM
Please fill in respondent details.
1:Name *SR *USEGRIDLIST "NameList"
2:Age *NUMBER
3:City *DROPDOWN *USEGRIDLIST "CityList"
```

**Common Modifiers:** `*SHOWASFORM`, `*MANDATORY`

**Notes:**
- Each attribute inside a `*FORM` must specify its own sub-type (`*SR`, `*MR`, `*DROPDOWN`, `*AUTOCOMPLETE`)
- `*SHOWASFORM` on another question type (e.g., `*SR *SHOWASFORM`) sets `NumberOfColumn = "3"` to render that question in form style
- Keyword detection explicitly excludes `*SHOWASFORM` from triggering the FORM type

---

### Type 49 — Info (`*INFO`)

**QType ID:** `49`
**Keyword:** `*INFO`

Displays a screen of text/images to the respondent without collecting any response. Used for instructions, introductions, or section headers.

**Syntax:**
```
*QUESTION IntroScreen *INFO *DELAY 3000
Welcome to the survey. This should take about 10 minutes.
```

**Common Modifiers:** `*DELAY`, `*PICT "path"`, `*VIDEO "path"`, `*NOBACKBTN`, `*NONEXTBTN`

**Notes:**
- No attributes or option list
- `*DELAY n` auto-advances after n milliseconds (useful for splash screens)
- `*NONEXTBTN` hides the Next button (combine with `*DELAY` for auto-advance)

---

### Type 50 — End (`*END`)

**QType ID:** `50`
**Keyword:** `*END`

Marks the successful completion point of the survey.

**Syntax:**
```
*QUESTION SurveyEnd *END
Thank you for completing the survey!
```

**Notes:**
- Sets the internal `hasEnd = true` flag — exactly one `*END` is required per script
- Typically the last question in the script
- No attributes needed

---

### Type 51 — Terminate (`*TERMINATE`)

**QType ID:** `51`
**Keyword:** `*TERMINATE`

Marks an early termination / screen-out point. Used when a respondent does not qualify.

**Syntax:**
```
*QUESTION ScreenOut *TERMINATE
Thank you for your time. You do not qualify for this study.
```

**Notes:**
- Sets the internal `hasTerminate = true` flag
- A script can have multiple `*TERMINATE` questions (e.g., ScreenOut1, ScreenOut2)
- Typically reached via `*IF [...] *GOTO ScreenOut` logic

---

### Type 60 — FIFS (`*FIFS`)

**QType ID:** `60`
**Keyword:** `*FIFS`

**Field Implementation & Field Supervisor** — a special system question that auto-generates four mandatory text-entry fields for field management data.

**Syntax:**
```
*QUESTION FIFSInfo *FIFS
Field Interviewer and Supervisor Details
```

**Auto-Generated Attributes (hardcoded):**

| Value | Label | Type | Mandatory |
|-------|-------|------|-----------|
| 1 | FI Name | ALPHA (LinkId1=3) | Yes |
| 2 | FI Code | ALPHA (LinkId1=3) | Yes |
| 3 | FS Name | ALPHA (LinkId1=3) | Yes |
| 4 | FS Code | ALPHA (LinkId1=3) | Yes |

**Notes:**
- This question is **mandatory** in every script — the compiler checks for `FIFSInfo`
- All four attributes are auto-created; do not add manual attributes
- The QId `FIFSInfo` is the required standard identifier

---

## Mandatory System Questions

Every script **must** include these four QIds or the compiler will report errors:

| QId | Purpose |
|-----|---------|
| `RespName` | Respondent name |
| `RespMobile` | Respondent mobile number |
| `Centre` | Centre/location identifier |
| `FIFSInfo` | Field Interviewer & Supervisor details (`*FIFS`) |

---

## Quick Reference Table

| Type ID | Keyword | Category | Has Attributes | Grid Required |
|---------|---------|----------|----------------|---------------|
| 1 | `*SR` | Choice | Yes | No |
| 2 | `*MR` | Choice | Yes | No |
| 3 | `*OPEN` | Text | No | No |
| 4 | `*NUMBER` | Numeric | No | No |
| 5 | `*RANK` | Order | Yes | No |
| 6 | `*IMAGE` | Media | No | No |
| 7 | `*GRIDSR` | Grid | Yes (rows) | Yes |
| 8 | `*GRIDMR` | Grid | Yes (rows) | Yes |
| 9 | `*MEDIA` | Media | No | No |
| 10 | `*RECORDING` | Capture | No | No |
| 12 | `*ALPHALIST` | Text List | Yes | No |
| 13 | `*NUMLIST` | Numeric List | Yes | No |
| 14 | `*DATE` | Date/Time | No | No |
| 15 | `*TIME` | Date/Time | No | No |
| 16 | `*CAPTUREIMAGE` | Capture | No | No |
| 17 | `*NUMLISTTOTAL` | Numeric List | Yes | No |
| 22 | `*AUTOCOMPLETE` / `*AUTOCOMPLETELIST` | Choice | Optional | Optional |
| 23 | `*AUTOCOMPLETEANS` | Choice | No | No |
| 24 | `*DROPDOWN` / `*DROPDOWNLIST` | Choice | Optional | Optional |
| 26 | `*DRAGDROP` | Order | Yes (rows) | Yes |
| 27 | `*GRIDNUM` | Grid | Yes (rows) | Yes |
| 32 | `*PSCALE` | Grid | Yes (rows) | Yes |
| 40 | `*MAXDIFF` | Scale | Yes | No |
| 41 | `*GPS` | Capture | Optional | No |
| 48 | `*FORM` | Layout | Yes | Optional |
| 49 | `*INFO` | Display | No | No |
| 50 | `*END` | Control | No | No |
| 51 | `*TERMINATE` | Control | No | No |
| 60 | `*FIFS` | System | Auto | No |
