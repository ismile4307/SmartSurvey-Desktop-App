<?php
    //open connection to mysql db
    $connection = mysqli_connect("localhost","survfiqz_ismile","Arnisha@4307#","survfiqz_surveyhive") or die("Error " . mysqli_error($connection));

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
   $query = "SELECT answers_".$projectCode.".`id` , answers_".$projectCode.".`interview_info_id` , answers_".$projectCode.".`project_id` , answers_".$projectCode.".`respondent_id` , answers_".$projectCode.".`q_id` , answers_".$projectCode.".`response` , answers_".$projectCode.".`responded_at` , answers_".$projectCode.".`q_elapsed_time` , answers_".$projectCode.".`q_order` , answers_".$projectCode.".`resp_order` , answers_".$projectCode.".`created_at` , answers_".$projectCode.".`deleted_at`
			FROM  answers_".$projectCode." INNER JOIN interview_infos_".$projectCode." ON answers_".$projectCode.".`interview_info_id` = interview_infos_".$projectCode.".`id`
          	WHERE  interview_infos_".$projectCode.".project_id=".$projectCode." AND interview_infos_".$projectCode.".survey_start_at BETWEEN '".$startDate."' AND '".$endDate."' AND interview_infos_".$projectCode.".intv_type='".$interviewType."' AND interview_infos_".$projectCode.".`status`!='4' AND interview_infos_".$projectCode.".deleted_at IS NULL LIMIT 10000 OFFSET ".$offset;
          }else{
            $query = "SELECT answers_".$projectCode.".`id` , answers_".$projectCode.".`interview_info_id` , answers_".$projectCode.".`project_id` , answers_".$projectCode.".`respondent_id` , answers_".$projectCode.".`q_id` , answers_".$projectCode.".`response` , answers_".$projectCode.".`responded_at` , answers_".$projectCode.".`q_elapsed_time` , answers_".$projectCode.".`q_order` , answers_".$projectCode.".`resp_order` , answers_".$projectCode.".`created_at` , answers_".$projectCode.".`deleted_at`
      FROM  answers_".$projectCode." INNER JOIN interview_infos_".$projectCode." ON answers_".$projectCode.".`interview_info_id` = interview_infos_".$projectCode.".`id`
            WHERE  interview_infos_".$projectCode.".project_id=".$projectCode." AND interview_infos_".$projectCode.".intv_type='".$interviewType."' AND interview_infos_".$projectCode.".created_at BETWEEN '".$startDate."' AND '".$endDate."' AND interview_infos_".$projectCode.".`status`!='4' AND interview_infos_".$projectCode.".deleted_at IS NULL LIMIT 10000 OFFSET ".$offset;
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