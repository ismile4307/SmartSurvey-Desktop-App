   M S B A M L   `   `   `  ����  DBI Scripting    DBI_Scripting.MainWindow   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5        1  +System.Windows.Controls.Ribbon.RibbonWindow   Name$  frmMainWindow��5       � Title$ Smart Survey (v1.0.0.2)��5   	   $��876.886��6)   $��1528.566��6:    � WindowState$ 	Maximized=�6K    � Icon$ Images/ic_surveybd2.png��6�   .��5      � $��0,0,0,0q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      .+�5   
   +  %System.Windows.Controls.Ribbon.Ribbon  -   5          Name$ 	RibbonWin��5       �� SelectedIndex$ 0��6%      HelpPaneContent 5      1  +System.Windows.Controls.Ribbon.RibbonButton  5         SmallImageSource$ Images\help32.png��6   	   QuickAccessToolBar	 5      =  7System.Windows.Controls.Ribbon.RibbonQuickAccessToolBar  5      .@�5        -   5        
  Name$

 Save��5   #   $ Images\PPS32.png��62   6  0System.Windows.Controls.Ribbon.RibbonSplitButton  5      -   5          Name$
 Undo��5   (   5  /System.Windows.Controls.Ribbon.RibbonMenuButton   SmallImageSource$ Images\PPS_32.png��67   .@�5      8  2System.Windows.Controls.Ribbon.RibbonSplitMenuItem  $��Undo 1��6.     5      $��Undo 2��6.     5      $��Undo 3��6.   5        5      -   5        $
 Redo��5   (   $&  Images\Folder-Blue-Apps-icon.png��66   .@�5        $��Redo 1��6.     5      $��Redo 2��6.   5      5         ApplicationMenu 5!      :  4System.Windows.Controls.Ribbon.RibbonApplicationMenu  5"         KeyTip$ F��6(   .@�5#      >	  8System.Windows.Controls.Ribbon.RibbonApplicationMenuItem	  $��Options��60   3
  -System.Windows.Controls.Ribbon.RibbonMenuItem 
  ImageSource$ Images\setBrandCode32.png��6A   	  5$      $
��Exit��60   $ Images\exit32.png��6>   5%      .@�5,      .  (System.Windows.Controls.Ribbon.RibbonTab  $��	Scripting��6      KeyTip$ H��6+   .@�5.      0  *System.Windows.Controls.Ribbon.RibbonGroup  -   5          Name$ ScriptingGroup1��5.      $
��Home��67   .@�5/        -   5           LargeImageSource$ Images\Notebook132.png��5/   #      Label$ Paste��6M      KeyTip$ V��6s     51      $ Images\Notebook232.png��6#   $	 Cut��6M   $ X��6Y     52      -   5        $
 btn_Copy��52   #   $ Images\Placeholder32.png��65   $
 Copy��6a   $ C��6n   6y   54        56      -	   5        $ ScriptingGroup2��56      $��	Scripting��67   .@�5;        -
   5        $
 btnPrepareLoopSyntax��5;   #   $" Images\create_project-48.png��6A   $ Loop Syntax��6q   $ V��6�   6�     5<      -   5        $
 btnBuildScript��5<   #   $! Images\build_project-62.png��6;   $ Build Script��6j   $ V��6   6�     5>      -   5        $
 btnUploadScript��5>   #   $! Images\upload_script-64.png��6<   $ Upload Script��6k   $ X��6�   6�     5?      -   5        $
 btnUploadMedia��5?   #   $  Images\upload_media-64.png��6;   $ Upload Media��6i   $ X��6~   6�     5@      -   5        $
 btnAddPanelData��5@   #   $ Images\server_50.png��6<   $ 	Add PData��6d   $ L��6v   6�     5A      -   5        $
 btnPlaceholder2��5A   #   $ Images\language.png��6<   $ Place Holder��6c   $ L��6x   6�   5B        5C      -   5        $ ScriptingGroup3��5C      $��	Analytics��67   .@�5H        -   5        $
 btnCreateSPSSSyntax��5H   #   $ Images\spss.png��6@   $ SPSS Script��6c   6w     5I      -   5        $
 btnDummyData��5I   #   $ Images\Notebook132.png��69   $ 
Dummy Data��6c   6v   5M      5N        5W      $��Download��6   $ I��6*   .@�5Y        -   5        $ Download1Group��5Y      $��Download��66   .@�5Z        -   5        $
 BtnDownloadData��5Z   #   $ Images/download_48.png��6<   $
 Data��6f   $ V��6s   6~     5\      -   5        $
 btnDownloadMedia��5\   #   $ Images\Notebook232.png��6=   $ Media��6g   $ X��6u   6�     5]      -   5        $
 btnDownloadScript��5]   #   $ Images\Placeholder32.png��6>   $ Script��6j   $ C��6y   6�     5^      -   5        $
 BtnDownloadTime��5^   #   $ Images/download_48.png��6<   $
 Time��6f   $ V��6s   6~   5`        5b      -   5        $ Download2Group��5b      $��Quality Control��66   .@�5g        -   5        $
 btnRLD��5g   #   $ Images\Open32.png��63   $	 RLD��6X   $ V��6d   6o     5h      -   5        $
 btnRejectInterview��5h   #   $ Images\reject_64.png��6?   $ Reject��6g   $ X��6v   6�     5i      -   5        $
 btnUpdateRLD��5i   #   $ Images\Script132.png��69   $ Update��6a   $ C��6p   6{   5j        5l      -   5        $ Download3Group��5l      $��OE Management��66   .@�5q        -   5        $
 btnCreateOEExcel��5q   #   $ Images\Open32.png��6=   $	 OEM��6b   $ V��6n   6y     5r      -   5        $
 btnCreateSyntax��5r   #   $ Images\checkData32.png��6<   $ Syntax��6f   $ X��6u   6�     5s      -   5        $
 btnUnPivotOE��5s   #   $ Images\Script132.png��69   $ UnPivot��6a   $ C��6q   6|   5t        5u      -    5        $ Download4Group��5u      $��Settings��66   .@�5v        -!   5        $
 btnSyncData��5v   #   $ Images\add_project_48.png��68   $ SyncData��6e   $ V��6v   6�     5w      -"   5        $
 btnPrepareAnsDBFromServer��5w   #   $ Images\Placeholder32.png��6F   $ AnsDB��6r   $ V��6�   6�     5x      $ Images\settings_60.png��6#   $ Settings��6M   $ V��6^   5y      5z        5�      $��	Analytics��6   $ L��6+   .@�5�        -#   5        $ AnalyticsGroup1��5�      $��SPSS Syntax��67   .@�5�        -$   5        $
 btnSPSSLabelToCode��5�   #   $ Images\Notebook132.png��6?   $ Label To Code��6i   $ V��6   6�     5�      -%   5        $
 btnTranspose��5�   #   $ Images\Notebook232.png��69   $ 	Transpose��6c   $ X��6u   6�     5�      $ Images\Placeholder32.png��6#   $ 	MR Syntax��6O   $ C��6a   5�        5�      -&   5        $ AnalyticsGroup2��5�      $��Analysis��67   .@�5�        -'   5        $
 btnAnalyticsStructure��5�   #   $ Images\Excel_17559_32.png��6B   $ Analysis Structure��6o   $ V��6�   6�     5�      -(   5        $
 btnTableSyntax��5�   #   $ Images\getBrandCode32.png��6;   $ Table Syntax��6h   $ V��6}   6�     5�      -)   5        $
 
btnSigTest��5�   #   $ Images\getPromoCode32.png��67   $ Sig Test��6d   $ V��6u   6�     5�      -*   5        $
 btnCETableLink��5�   #   $ Images\Table132.png��6;   $ 
Table Link��6b   $ X��6u   6�     5�      -+   5        $
 btnGetQuestionnaire��5�   #   $&  Images\Folder-Blue-Apps-icon.png��6@   $ Compare SPSS��6t   $ V��6�   6�   5�        5�      -,   5        $ AnalyticsGroup3��5�      $��OE Management��67   .@�5�        --   5        $
 btnOESyntaxSPSS��5�   #   $  Images\upload_media-64.png��6<   $ 	OE Syntax��6j   $ X��6|   6�     5�      -.   5        $
 	btnOEBill��5�   #   $ Images\language.png��66   $ OE Bill��6]   $ L��6m   6x     5�      -/   5        $
 btnUpdateSyntaxPrep��5�   #   $ Images\editData32.png��6@   $ Update Syntax��6i   $ L��6   6�     5�      -0   5        $
 btnOEUnPivot��5�   #   $ Images\Script132.png��69   $ Un Pivot OESyntax��6a   $ C��6{   6�   5�        5�      -1   5        $ AnalyticsGroup4��5�      $��CTable Analysis��67   .@�5�        -2   5        $
 btnCumulativeSyntax��5�   #   $ Images\Excel_17559_32.png��6@   $ Cumulative Syntax��6m   $ V��6�   6�     5�      -3   5        $
 btnCTableSyntax��5�   #   $ Images\getBrandCode32.png��6<   $ CTable Syntax��6i   $ V��6     5�      -4   5        $
 btnCTableLink��5�   #   $ Images\Table132.png��6:   $ CTable Link��6a   $ X��6u   6�     5�      -5   5        $

 btn4��5�   #   $&  Images\Folder-Blue-Apps-icon.png��61   $ Compare SPSS��6e   $ V��6z   5�      5�        5�      $��
Automation��6   $ L��6,   5�        5�      $��
Web portal��6   $ L��6,   .@�5�        -6   5        $ 	WebGroup1��5�      $��Table Syntax Preparation��61   .@�5�        -7   5        $
 btnDbStructure��5�   #   $ Images\language.png��6;   $ DB Structure��6b   $ L��6w   6�     5�      -8   5        $
 btnQntrTable��5�   #   $ Images\Open32.png��69   $ Question Table��6^   $ L��6u   6�     5�      -9   5        $
 
btnSRTable��5�   #   $" Images\create_project-48.png��67   $ SR_Table��6g   $ V��6x   6�     5�      -:   5        $
 
btnMRTable��5�   #   $! Images\build_project-62.png��67   $ MR_Table��6f   $ V��6w   6�     5�      -;   5        $
 btnAttribute��5�   #   $! Images\upload_script-64.png��69   $ 	Attribute��6h   $ X��6z   6�   5�      5�        5�      $��Settings��6   $ L��6*   .@�5�        -<   5        $ SettingsGroup��5�      $��Download��65   .@�5�        -=   5        $
 btnSettingsServer��5�   #   $ Images/server_50.png��6>   $ Server��6f   $ V��6u   6�     5�      ->   5        $
 btnLogin��5�   #   $ Images\settings_60.png��65   $ Login��6_   $ V��6m   6x     5�      $ Images\Notebook232.png��6#   $ Media��6M   $ X��6[     5�      $ Images\Placeholder32.png��6#   $ Script��6O   $ C��6^   5�      5�        5�      $
��Help��6   $ L��6&   5�      5�      �� 5�   
   $
��Left=�6   $	��300��6+   $��526,38.667,0,0q�68   $��1��6P   $	��Top=�6]   $	��300��6u   $��Images/ic_surveybd2.png��6�   �� 5�   
   $$��Smart Survey Management System��6   $
��Left=�69   $��291,357.667,0,0q�6T   $��1��6m   $	��Top=�6z   $��79��6�   $	��757��6�   $��48(�6�   �� 5�   
   -?   5         ��Name$ lblServerName��5�      $��Server URL : ��6'   $
��Left=�6?   $��375,436.667,0,0q�6Z   $��1��6s   $	��Top=�6�   $��30(�6�   $	��741��6�   
������ ���	#FF1E0FED6�   5�      5�      