<?php
include("include/db_connect.php");


$file = 'SYSACDB.db';
$newfile = 'bakupdb.db';

if (!copy($file, $newfile)) {
    echo "failed to copy $file...\n";
}
else{
    //echo "backup completed";
}

class MyDB extends SQLite3
{
 function __construct()
  {
     $this->open('bakupdb.db');
  }
}

//$db = new MyDB();
/*
$db = new MyDB();
   if(!$db){
      echo $db->lastErrorMsg();
   } else {
      //echo "Opened database successfully\n";
   }
*/


   $startDate = "2017-10-14 00:00:00";
   $endDate = "2017-10-25 23:59:59";
   //$dateType= $_POST['dateType'];
   $projectCode= '23817';


   DB_Util::connect();
   
   //echo $connection;
   
   //$query = "SELECT project_id,respondent_id,latitude,longitude,survey_start_at,survey_end_at,length_of_intv,intv_type,fi_code,fs_code,accompanied_by,back_checked_by,status,tab_id,sync_status,script_version,language_id,field_ex1,field_ex2 FROM `interview_infos` WHERE project_id=".$projectCode." AND survey_start_at BETWEEN '" . $startDate . "' AND '" . $endDate . "' AND deleted_at IS NULL";
   
   $query = "SELECT * FROM `interview_infos` WHERE project_id=".$projectCode." AND survey_start_at BETWEEN '" . $startDate . "' AND '" . $endDate . "' AND deleted_at IS NULL";
   
   //echo $query;
   
   $result = mysql_query($query);// or die("Error in Selecting " . mysqli_error($connection));
   //create an array
   //$emparray = array();
   while($row =mysql_fetch_assoc($result))
   {
   	$emparray=array();

        $emparray['ProjectId'] = $row['project_id'];
	$emparray['RespondentId'] = $row['respondent_id'];
	$emparray['Latitude'] = $row['latitude'];
	$emparray['Longitude'] = $row['longitude'];
	$emparray['SurveyDateTime'] = $row['survey_start_at'];
	$emparray['SurveyEndTime'] = $row['survey_end_at'];
	$emparray['LengthOfIntv'] = $row['length_of_intv'];
	$emparray['Intv_Type'] = $row['intv_type'];
	$emparray['FICode'] = $row['fi_code'];
	$emparray['FSCode'] = $row['fs_code'];
	$emparray['AccompaniedBy'] = $row['accompanied_by'];
	$emparray['BackCheckedBy'] = $row['back_checked_by'];
	$emparray['Status'] = $row['status'];
	$emparray['TabId'] = $row['tab_id'];
	$emparray['SyncStatus'] = $row['sync_status'];
	$emparray['ScriptVersion'] = $row['script_version'];
	$emparray['LanguageId'] = $row['language_id'];
	$emparray['FieldExtra1'] = $row['field_ex1'];
	$emparray['FieldExtra2'] = $row['field_ex2'];

	//$emparray[] = $row;

	insert_interviewinfo('T_InterviewInfo',$emparray);
	
   //echo json_encode($emparray);

   }

////****************************************************************

 $query = $query = "SELECT answers.`id` , answers.`interview_info_id` , answers.`project_id` , answers.`respondent_id` , answers.`q_id` , answers.`response` , answers.`responded_at` , answers.`q_elapsed_time` , answers.`q_order` , answers.`resp_order` , answers.`created_at` , answers.`deleted_at`
			FROM  `answers` INNER JOIN interview_infos ON answers.`interview_info_id` = interview_infos.`id`
          	WHERE  interview_infos.project_id=".$projectCode." AND interview_infos.survey_start_at BETWEEN '".$startDate."' AND '".$endDate."' AND interview_infos.deleted_at IS NULL";
          	
   
   $result = mysql_query($query);// or die("Error in Selecting " . mysqli_error($connection));
   //create an array
   //$emparray = array();
   while($row =mysql_fetch_assoc($result))
   {
   	$emparray=array();

        $emparray['ProjectId'] = $row['project_id'];
	$emparray['RespondentId'] = $row['respondent_id'];
	
	$emparray['QId'] = $row['q_id'];
	$emparray['Response'] = $row['response'];
	$emparray['ResponseDateTime'] = $row['responded_at'];
	$emparray['qElapsedTime'] = $row['q_elapsed_time'];
	$emparray['qOrderTag'] = $row['q_order'];
	$emparray['rOrderTag'] = $row['resp_order'];
	
	//$emparray[] = $row;

	insert_interviewinfo('T_RespAnswer',$emparray);
	
   //echo json_encode($emparray);

   }


//******************************************************************  

////****************************************************************

$query = "SELECT open_endeds.`id` , open_endeds.`interview_info_id` , open_endeds.`project_id` , open_endeds.`respondent_id` , open_endeds.`q_id` , open_endeds.`attribute_value` , open_endeds.`response` , open_endeds.`response_type` , open_endeds.`created_at` , open_endeds.`deleted_at`
			FROM  `open_endeds` INNER JOIN interview_infos ON open_endeds.`interview_info_id` = interview_infos.`id`
          	WHERE  interview_infos.project_id=".$projectCode." AND interview_infos.survey_start_at BETWEEN '".$startDate."' AND '".$endDate."' AND interview_infos.deleted_at IS NULL";
          	
    $result = mysql_query($query);// or die("Error in Selecting " . mysqli_error($connection));
   //create an array
   //$emparray = array();
   while($row =mysql_fetch_assoc($result))
   {
   	$emparray=array();

        $emparray['ProjectId'] = $row['project_id'];
	$emparray['RespondentId'] = $row['respondent_id'];
	
	$emparray['QId'] = $row['q_id'];
	$emparray['AttributeValue'] = $row['attribute_value'];
	$emparray['OpenendedResp'] = $row['response'];
	$emparray['OEResponseType'] = $row['response_type'];
	
	//$emparray[] = $row;

	insert_interviewinfo('T_RespOpenended',$emparray);
	
   //echo json_encode($emparray);

   }

echo "backup completed";
//******************************************************************           	
          	
          	
          	
          	 
//   echo json_encode($emparray);

   function insert_interviewinfo($tableName, $params)
    {
        $query = "INSERT INTO " . $tableName . " (";
        $values = " VALUES ( ";
        $len = count($params);
        $i = 0;
        foreach ($params as $key => $value) {
            if (++$i != $len) {
                $query .= $key . ", ";
                $values .= "'" . $value . "', ";
            } else {
                $query .= $key . ") ";
                $values .= "'" . $value . "')";
            }
        }
        $query .= $values;
        
        //echo $query;
        $db = new MyDB();
        $result = $db->exec($query);

        
    }

?>

