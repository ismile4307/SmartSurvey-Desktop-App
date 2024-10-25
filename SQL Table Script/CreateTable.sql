--
-- Table structure for table `interview_infos`
--

CREATE TABLE `interview_infos_40528` (
  `id` int(10) UNSIGNED NOT NULL,
  `project_id` int(10) UNSIGNED NOT NULL,
  `respondent_id` bigint(20) NOT NULL,
  `latitude` varchar(20) COLLATE utf8_unicode_ci NOT NULL,
  `longitude` varchar(20) COLLATE utf8_unicode_ci NOT NULL,
  `survey_start_at` datetime NOT NULL,
  `survey_end_at` datetime NOT NULL,
  `length_of_intv` varchar(10) COLLATE utf8_unicode_ci NOT NULL,
  `intv_type` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `fi_code` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `fs_code` varchar(10) COLLATE utf8_unicode_ci NOT NULL,
  `accompanied_by` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `back_checked_by` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `status` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `tab_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `sync_status` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `script_version` varchar(20) COLLATE utf8_unicode_ci NOT NULL,
  `language_id` varchar(10) COLLATE utf8_unicode_ci NOT NULL,
  `field_ex1` varchar(50) COLLATE utf8_unicode_ci NOT NULL,
  `field_ex2` varchar(20) COLLATE utf8_unicode_ci NOT NULL,
  `created_at` datetime DEFAULT NULL,
  `deleted_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `interview_infos`
--
ALTER TABLE `interview_infos_40528`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `interview_infos`
--
ALTER TABLE `interview_infos_40528`
  MODIFY `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT;
COMMIT;

--
-- Table structure for table `answers`
--

CREATE TABLE `answers_40528` (
  `id` int(10) UNSIGNED NOT NULL,
  `interview_info_id` int(10) UNSIGNED NOT NULL,
  `project_id` int(10) UNSIGNED NOT NULL,
  `respondent_id` bigint(20) NOT NULL,
  `q_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `response` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `responded_at` datetime NOT NULL,
  `q_elapsed_time` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `q_order` int(11) NOT NULL,
  `resp_order` int(11) NOT NULL,
  `created_at` datetime DEFAULT NULL,
  `deleted_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `answers`
--
ALTER TABLE `answers_40528`
  ADD PRIMARY KEY (`id`),
  ADD KEY `DateIndex` (`created_at`),
  ADD KEY `InterviewIDIndex` (`interview_info_id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `answers`
--
ALTER TABLE `answers_40528`
  MODIFY `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT;
COMMIT;


--
-- Table structure for table `open_endeds`
--

CREATE TABLE `open_endeds_40528` (
  `id` int(10) UNSIGNED NOT NULL,
  `interview_info_id` int(10) UNSIGNED NOT NULL,
  `project_id` int(10) UNSIGNED NOT NULL,
  `respondent_id` bigint(20) NOT NULL,
  `q_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `attribute_value` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `response` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `response_type` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `created_at` datetime DEFAULT NULL,
  `deleted_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `open_endeds`
--
ALTER TABLE `open_endeds_40528`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `open_endeds`
--
ALTER TABLE `open_endeds_40528`
  MODIFY `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT;
COMMIT;