<?php
include("include/db_connect.php");

DB_Util::connect();        

$query="SELECT * FROM `interview_infos` WHERE `project_id`=23817 AND `intv_type`='1' AND `survey_start_at` BETWEEN '2017-10-21 00:00:00' AND '2017-10-21 23:59:59' AND `deleted_at` IS NULL";



$sql=mysql_query($query);

$posts=Array();

while($rows1 = mysql_fetch_assoc($sql)) {

	$rowsinfo=Array();
	
	$interviewerid=$rows1['id'];
	//$rowsinfo[]=$rows1;
	
	$rowsinfo['id']=$rows1['id'];
	$rowsinfo['project_id']=$rows1['project_id'];
	$rowsinfo['respondent_id']=$rows1['respondent_id'];
	$rowsinfo['latitude']=$rows1['latitude'];
	$rowsinfo['longitude']=$rows1['longitude'];
	$rowsinfo['survey_start_at']=$rows1['survey_start_at'];
	$rowsinfo['survey_end_at']=$rows1['survey_end_at'];
	$rowsinfo['length_of_intv']=$rows1['length_of_intv'];
	$rowsinfo['intv_type']=$rows1['intv_type'];
	$rowsinfo['fi_code']=$rows1['fi_code'];
	$rowsinfo['fs_code']=$rows1['fs_code'];
	$rowsinfo['accompanied_by']=$rows1['accompanied_by'];
	$rowsinfo['back_checked_by']=$rows1['back_checked_by'];
	$rowsinfo['status']=$rows1['status'];
	$rowsinfo['tab_id']=$rows1['tab_id'];
	$rowsinfo['sync_status']=$rows1['sync_status'];
	$rowsinfo['script_version']=$rows1['script_version'];
	$rowsinfo['language_id']=$rows1['language_id'];
	$rowsinfo['field_ex1']=$rows1['field_ex1'];
	$rowsinfo['field_ex2']=$rows1['field_ex2'];
	$rowsinfo['created_at']=$rows1['created_at'];
	$rowsinfo['deleted_at']=$rows1['deleted_at'];
	
	$response_query = mysql_query("SELECT * FROM `answers` WHERE `interview_info_id`=".$interviewerid." AND `project_id`=23817");
	
	$posts1=Array();
	while($rows2= mysql_fetch_array($response_query )){
	$rowsresp=Array();
		$rowsresp['id']=$rows2['id'];
		$rowsresp['interview_info_id']=$rows2['interview_info_id'];
		$rowsresp['project_id']=$rows2['project_id'];
		$rowsresp['respondent_id']=$rows2['respondent_id'];
		$rowsresp['q_id']=$rows2['q_id'];
		$rowsresp['response']=$rows2['response'];
		$rowsresp['responded_at']=$rows2['responded_at'];
		$rowsresp['q_elapsed_time']=$rows2['q_elapsed_time'];
		$rowsresp['q_order']=$rows2['q_order'];
		$rowsresp['resp_order']=$rows2['resp_order'];
		
		$posts1[]=$rowsresp;
		
		//$jsonresp=json_encode($rowsresp);
	}
	
	$oe_query = mysql_query("SELECT * FROM `open_endeds` WHERE `interview_info_id`=".$interviewerid." AND `project_id`=23817");
	
	$posts2=Array();
	while($rows3= mysql_fetch_array($response_query )){
	$rowsoe=Array();
		$rowsoe['id']=$rows3['id'];
		$rowsoe['interview_info_id']=$rows3['interview_info_id'];
		$rowsoe['project_id']=$rows3['project_id'];
		$rowsoe['respondent_id']=$rows3['respondent_id'];
		$rowsoe['q_id']=$rows3['q_id'];
		$rowsoe['attribute_value']=$rows3['attribute_value'];
		$rowsoe['response']=$rows3['response'];
		$rowsoe['response_type']=$rows3['response_type'];
		
		$posts2[]=$rowsoe;
	}
	
	$posts[] = array('interviewinfo'=>$rowsinfo, 'response'=> $posts1, 'openended'=> $posts2);

}

echo json_encode(Array('querydata'=>$posts));



//$fp = fopen('results.json', 'w');
//fwrite($fp, json_encode($posts));
//fclose($fp);

?>