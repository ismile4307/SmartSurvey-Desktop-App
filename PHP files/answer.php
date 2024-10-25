<?php
    //open connection to mysql db
    $connection = mysqli_connect("localhost","survfiqz_ismile","Arnisha@4307#","survfiqz_capi") or die("Error " . mysqli_error($connection));

   ini_set('memory_limit', '1024M');
    //fetch table rows from mysql db
   $startDate = $_POST['startDate'] . " 00:00:00";
   $endDate = $_POST['endDate'] . " 23:59:59";
   $dateType= $_POST['dateType'];
   $projectCode= $_POST['projectCode'];
   $offset=$_POST['myOffset'];
   $interviewType=$_POST['interviewType'];


    //Here $dateType=1 means Interview date
    //$dateType=2 means Sync Date

   IF ($dateType=="1"){
   $query = "SELECT answers.`id` , answers.`interview_info_id` , answers.`project_id` , answers.`respondent_id` , answers.`q_id` , answers.`response` , answers.`responded_at` , answers.`q_elapsed_time` , answers.`q_order` , answers.`resp_order` , answers.`created_at` , answers.`deleted_at`
			FROM  `answers` INNER JOIN interview_infos ON answers.`interview_info_id` = interview_infos.`id`
          	WHERE  interview_infos.project_id=".$projectCode." AND interview_infos.survey_start_at BETWEEN '".$startDate."' AND '".$endDate."' AND interview_infos.intv_type='".$interviewType."' AND interview_infos.deleted_at IS NULL LIMIT 10000 OFFSET ".$offset;
          }else{
            $query = "SELECT answers.`id` , answers.`interview_info_id` , answers.`project_id` , answers.`respondent_id` , answers.`q_id` , answers.`response` , answers.`responded_at` , answers.`q_elapsed_time` , answers.`q_order` , answers.`resp_order` , answers.`created_at` , answers.`deleted_at`
      FROM  `answers` INNER JOIN interview_infos ON answers.`interview_info_id` = interview_infos.`id`
            WHERE  interview_infos.project_id=".$projectCode." AND interview_infos.intv_type='".$interviewType."' AND interview_infos.created_at BETWEEN '".$startDate."' AND '".$endDate."' AND interview_infos.deleted_at IS NULL LIMIT 10000 OFFSET ".$offset;
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