   M S B A M L   `   `   `  ����  DBI Scripting4    .DBI_Scripting.Forms.Scripting.FrmCreateProject   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          �Name$  frmCreateProject��5   	    � Title$ Create Project��5   	   $��497.395��6    $��785.34��61    � WindowStartupLocation$ CenterScreen=�6@   .��5      � $��0,0,0.667,20q�6   .+�5   
   �� $�� ��6   $
��Left=�6   $��	10,10,0,0q�68   $	��Top=�6K   $	��169��6c   $	��756��6p   .��5      � $��0,0,0,0.333q�6   .+�5      �� $��Project Name :��6   $
��Left=�61   $��5,6,0,0q�6L   $	��Top=�6]   �� 5	      $��Project Code :��6   $
��Left=�61   $��7,69,0,0q�6L   $	��Top=�6^   �� 5
      -   5         ��Name$ txtProjectName��5
      $
��Left=�62   $��23��6M   $��	96,10,0,0q�6Y    �� TextWrapping$
 Wrap=�6l   $�� ��6�   $	��Top=�6�   $	��226��6�   �� 5      -   5        $ txtProejctCode��5      $
��Left=�62   $��23��6M   $��	96,72,0,0q�6Y   $
 Wrap=�6l   $�� ��6�   $	��Top=�6�   $	��226��6�   �� 5      $��Script Name :��6   $
��Left=�60   $��	11,38,0,0q�6K   $	��Top=�6^   �� 5      $��Start Date : ��6   $
��Left=�60   $��
385,72,0,0q�6K   $	��Top=�6_    J� RenderTransformOrigin$ 0.518,2.496&�6w   �� 5      -   5        $ txtScriptFileName��5      $
��Left=�65   $��23��6P   $��	96,41,0,0q�6\   $
 Wrap=�6o   $�� ��6�   $	��Top=�6�   $	��226��6�   (  "System.Windows.Controls.DatePicker  5      -   5          Name$ dtpStartDate��5      $
��Left=�63   $��
457,72,0,0q�6N   $	��Top=�6b   $	��226��6z   �� 5      -   5         ��Name$ btnClose��5      $��Close��6+   $
��Left=�6;   $��608,115,0,0q�6V   $	��Top=�6k   $��75��6�   6�   �� 5      -   5        $ btnSave��5      $
��Save��6*   $
��Left=�69   $��507,115,0,0q�6T   $	��Top=�6i   $��75��6�   6�   �� 5      -   5        $ 
btnRefresh��5      $��Refresh��6-   $
��Left=�6?   $��406,115,0,0q�6Z   $	��Top=�6o   $��75��6�   �� 5      $��	Status : ��6   $
��Left=�6,   $��
41,104,0,0q�6G   $	��Top=�6[   �� 5      -	   5         ��Name$ comProjectStatus��5      $
��Left=�65   $��
96,108,0,0q�6P   $	��Top=�6d   $	��120��6|   �� 5      $��Media Version :��6   $
��Left=�62   $��
365,38,0,0q�6M   $	��Top=�6a   �� 5      -
   5        $ txtMediaVersion��5      $
��Left=�63   $��23��6N   $��
457,41,0,0q�6Z   $
 Wrap=�6n   $�� ��6�   $	��Top=�6�   $	��226��6�   �� 5      $��Script Version :��6   $
��Left=�63   $��	369,6,0,0q�6N   $	��Top=�6a   �� 5      -   5        $ txtScriptVersion��5      $
��Left=�64   $��23��6O   $��
457,10,0,0q�6[   $
 Wrap=�6o   $�� ��6�   $	��Top=�6�   $	��226��6�   5      5      �� 5   
   $�� ��6   $
��Left=�6   $��10,184,0,-14q�68   $	��Top=�6N   $	��278��6f   $	��756��6s   .��5      &   System.Windows.Controls.DataGrid  -   5        	  Name$	 dataGridProject��5      $
��Left=�60   $	��Top=�6K   $	��245��6c   $	��746��6p   $��0,0,-2,0q�6|   
   SelectionMode$
 Single=�6�   $ �� IsSynchronizedWithCurrentItem$
 TrueY�6�      CanUserSortColumns .     CanUserReorderColumns .     CanUserResizeRows .     CanUserAddRows .     
IsReadOnly . 6F  5      5!      5"      