   M S B A M L   `   `   `  ����  DBI Scripting.    (DBI_Scripting.Forms.Download.FrmSyncData   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  Sync Data to Server��5   	   $��451.086��6%   $��810.564��66    � WindowStartupLocation$ CenterScreen=�6F   .��5      � .+�5   
   �� $��Server Address :��6   $
��Left=�6+   $��	17,16,0,0q�6F   $	��Top=�6Y    J� RenderTransformOrigin$ -0.813,-0.45&�6q   $	��136��6�   �� 5   
   -   5         ��Name$ txtServerAddress��5      $
��Left=�6,   $��23��6G   $��
115,19,0,0q�6S    �� TextWrapping$
 Wrap=�6g   $�� ��6{   $	��Top=�6�   $	��668��6�   �� 5   
   $��Date From :��6   $
��Left=�6&   $��
289,49,0,0q�6A   $	��Top=�6U   $��94��6m   (  "System.Windows.Controls.DatePicker  5	   
   -   5          Name$ dtpDateFrom��5	      $
��Left=�6*   $��
363,51,0,0q�6E   $	��Top=�6Y   $	��175��6q   �� 5
   
   $��Project Name :��6   $
��Left=�6)   $��	19,49,0,0q�6D   $	��Top=�6W   $	��116��6o   �� 5   
   -   5         ��Name$ comProjectName��5      $
��Left=�6+   $��
115,53,0,0q�6F   $	��Top=�6Z   $	��172��6r   6�   �� 5   
   $��Data Type :��6   $
��Left=�6&   $��
539,49,0,0q�6A   $	��Top=�6U   $	��116��6m   �� 5   
   -   5        $ comDataType��5      $
��Left=�6(   $��
611,53,0,0q�6C   $	��Top=�6W   $	��172��6o   6{   �� 5   
   -   5         ��Name$ 	lblDBPath��5      $��Database Path :��6#   $
��Left=�6=   $��	17,82,0,0q�6X   $	��Top=�6k   $ -0.813,-0.45&�6�   $	��136��6�   �� 5   
   -   5        $ txtDataBasePath��5      $
��Left=�6+   $��23��6F   $��
115,85,0,0q�6R   $
 Wrap=�6f   $�� ��6z   $	��Top=�6�   $	��574��6�   �� 5   
   -   5         ��Name$ 	btnBrowse��5      $��Browse��6$   $
��Left=�65   $��
708,86,0,0q�6P   $	��Top=�6d   $��22��6|   $��75��6�   $ 1.35,-4.746&�6�   6�   &   System.Windows.Controls.DataGrid  5   
   -	   5        	  Name$	 dataGridView��5      $
��Left=�6)   $��
19,126,0,0q�6D   $	��Top=�6X   $	��233��6p   $	��764��6}   
   SelectionMode$
 Single=�6�      
IsReadOnly . ��5      �� 5       �� 
TargetType)  #System.Windows.Controls.DataGridRow"  6   .�5      -
   5        5      5      �� 5   
   $��Respondent Id :��6   $
��Left=�6*   $��
17,368,0,0q�6E   $	��Top=�6Y   $ -0.813,-0.45&�6q   $��98��6�   �� 5   
   -   5        $ txtRespondentId��5      $
��Left=�6+   $��23��6F   $��110,372,0,0q�6R   $
 Wrap=�6g   $�� ��6{   $	��Top=�6�   $	��152��6�   �� 5   
   -   5        $ btnSync��5      $
��Sync��6"   $
��Left=�61   $��384,373,0,0q�6L   $	��Top=�6a   $��22��6y   $��75��6�   $ 1.35,-4.746&�6�   6�   �� 5   
   -   5        $ lblMessageSyncData��5      $��	Message :��6,   $
��Left=�6@   $��473,369,0,0q�6[   $	��Top=�6p   $ -0.813,-0.45&�6�   $��98��6�   �� 5   
   -   5        $ txtNoOfRecord��5      $��No of Rec :��6'   $
��Left=�6=   $��269,369,0,0q�6X   $	��Top=�6m   $ -0.813,-0.45&�6�   $��98��6�   �� 5   
   -   5        $ btnClose��5      $
��Exit��6#   $
��Left=�62   $��708,373,0,0q�6M   $	��Top=�6b   $��22��6z   $��75��6�   $ 1.35,-4.746&�6�   6�   5      5       