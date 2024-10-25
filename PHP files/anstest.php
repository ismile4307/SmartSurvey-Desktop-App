<?php
    //open connection to mysql db
    $connection = mysqli_connect("localhost","chromsha_surveyhive","Arnisha@4307#","chromsha_capi") or die("Error " . mysqli_error($connection));

   ini_set('memory_limit', '1024M');
    //fetch table rows from mysql db
   $startDate = "2018-01-21 00:00:00";
   $endDate = "2018-01-21 23:59:59";
   $dateType= "1";
   $projectCode= "23817";

   IF ($dateType=="1"){
   $query = "SELECT answers.`id` , answers.`interview_info_id` , answers.`project_id` , answers.`respondent_id` , answers.`q_id` , answers.`response` , answers.`responded_at` , answers.`q_elapsed_time` , answers.`q_order` , answers.`resp_order` , answers.`created_at` , answers.`deleted_at`
			FROM  `answers` INNER JOIN interview_infos ON answers.`interview_info_id` = interview_infos.`id`
          	WHERE  interview_infos.project_id=".$projectCode." AND interview_infos.survey_start_at BETWEEN '".$startDate."' AND '".$endDate."' AND interview_infos.intv_type='1' AND interview_infos.deleted_at IS NULL LIMIT 9000";
          }else{
            $query = "SELECT answers.`id` , answers.`interview_info_id` , answers.`project_id` , answers.`respondent_id` , answers.`q_id` , answers.`response` , answers.`responded_at` , answers.`q_elapsed_time` , answers.`q_order` , answers.`resp_order` , answers.`created_at` , answers.`deleted_at`
      FROM  `answers` INNER JOIN interview_infos ON answers.`interview_info_id` = interview_infos.`id`
            WHERE  interview_infos.project_id=".$projectCode." AND interview_infos.intv_type='1' AND interview_infos.created_at BETWEEN '".$startDate."' AND '".$endDate."' AND interview_infos.deleted_at IS NULL";
          }

   $result = mysqli_query($connection, $query) or die("Error in Selecting " . mysqli_error($connection));

    //create an array
    $emparray = array();
    while($row =mysqli_fetch_assoc($result))
    {
        $emparray[] = $row;
    }
    echo json_encode($emparray);

    //close the db connection
    mysqli_close($connection);
?>