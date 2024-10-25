-- phpMyAdmin SQL Dump
-- version 4.9.7
-- https://www.phpmyadmin.net/
--
-- Host: localhost:3306
-- Generation Time: Feb 17, 2022 at 03:13 AM
-- Server version: 10.3.32-MariaDB-log-cll-lve
-- PHP Version: 7.3.32

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `survfiqz_capi`
--

-- --------------------------------------------------------

--
-- Table structure for table `interview_infos`
--

CREATE TABLE `interview_infos_220062` (
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
ALTER TABLE `interview_infos_220062`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `interview_infos`
--
ALTER TABLE `interview_infos_220062`
  MODIFY `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
