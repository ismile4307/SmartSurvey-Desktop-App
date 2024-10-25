<?php
// include("include/db_connect.php");

    //fetch table rows from mysql db
if (isset($_POST['startDate']))   
  $startDate = $_POST['startDate'] . " 00:00:00";

if (isset($_POST['endDate']))   
  $endDate = $_POST['endDate'] . " 23:59:59";

if (isset($_POST['ProjectId']))   
  $project_id= $_POST['ProjectId'];

if (isset($_POST['centreCode']))   
  $centreCode=$_POST['centreCode'];
   
//   echo "Ismile";
   
//DB_Util::connect();        
    $connection = mysqli_connect("localhost","survfiqz_ismile","Arnisha@4307#","survfiqz_surveyhive") or die("Error " . mysqli_error($connection));


$query1="SELECT * FROM `interview_infos_".$project_id."` WHERE `project_id`=".$project_id." AND `intv_type`='1' AND `status`='1' AND `survey_start_at` BETWEEN '".$startDate."' AND '".$endDate."' AND `deleted_at` IS NULL";

// $query1="SELECT * FROM `interview_infos_21007`";

// echo $query1;

$posts=Array();

$sql1=mysqli_query($connection,$query1);

    $emparray = array();
    
    while($rows1 =mysqli_fetch_assoc($sql1))
    {
        $rowsinfo=Array();
	
    	$interviewerid=$rows1['id'];
    	//$rowsinfo[]=$rows1;
    	
    	$rowsinfo['id']=$rows1['id'];
    	$rowsinfo['project_id']=$rows1['project_id'];
    	$rowsinfo['respondent_id']=$rows1['respondent_id'];
    	$rowsinfo['survey_start_at']=$rows1['survey_start_at'];
    	$rowsinfo['survey_end_at']=$rows1['survey_end_at'];
    	$rowsinfo['length_of_intv']=$rows1['length_of_intv'];
    	$rowsinfo['intv_type']=$rows1['intv_type'];
    	$rowsinfo['status']=$rows1['status'];
    	
    	//$response_query = mysqli_query($connection,"SELECT * FROM `answers_".$project_id."` WHERE `interview_info_id`=".$interviewerid." AND `project_id`=".$project_id);

    $response_query = mysqli_query($connection,"SELECT answer.respondent_id, answer.response, questions.display_jump_button AS address_order FROM `answers_".$project_id."` as answer INNER JOIN questions on answer.q_id=questions.qid WHERE (questions.display_jump_button='1' OR questions.display_jump_button='2' OR questions.display_jump_button='3') AND answer.`interview_info_id`=".$interviewerid." AND questions.`project_id`=".$project_id." ORDER BY answer.`interview_info_id`, questions.display_jump_button");

	$posts1=Array();
	while($rows2= mysqli_fetch_array($response_query )){
	$rowsresp=Array();
		$rowsresp['respondent_id']=$rows2['respondent_id'];
		$rowsresp['response']=$rows2['response'];
		$rowsresp['address_order']=$rows2['address_order'];
		
		$posts1[]=$rowsresp;
		
		//$jsonresp=json_encode($rowsresp);
	}

    
    $posts[] = array('interviewinfo'=>$rowsinfo, 'response'=> $posts1);
    // $posts[] = array('interviewinfo'=>$rowsinfo);

}

echo json_encode(Array('querydata'=>$posts));

    //close the db connection
    mysqli_close($connection);
	
	

//$fp = fopen('results.json', 'w');
//fwrite($fp, json_encode($posts));
//fclose($fp);

?>