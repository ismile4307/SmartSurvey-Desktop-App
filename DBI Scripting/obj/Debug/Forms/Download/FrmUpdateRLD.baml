   M S B A M L   `   `   `  ����  DBI Scripting/    )DBI_Scripting.Forms.Download.FrmUpdateRLD   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5   	     � Title$  FrmUpdateRLD��5   	   $��612.189��6   $��814.038��6/    � WindowStartupLocation$ CenterScreen=�6?    � 
ResizeMode$ NoResize=�6d   .��5      � .+�5   
   �� $��Source file Path :��6   $
��Left=�6-   $��	25,14,0,0q�6H   $	��Top=�6[    J� RenderTransformOrigin$ -3.74,-2.112&�6s   �� 5	   
   -   5         ��Name$ txtSourceFileLocation��5	      $
��Left=�61   $��23��6L   $��
122,17,0,0q�6X    �� TextWrapping$
 Wrap=�6l   $�� ��6�   $	��Top=�6�   $	��551��6�   �� 5
   
   -   5         ��Name$ btnBrowseSourceFile��5
      $��Browse��6.   $
��Left=�6?   $��
678,17,0,0q�6Z   $	��Top=�6n   $��75��6�   $ 8.079,-5.321&�6�   6�   �� 5   
   $��Source WrokSheet��6   $
��Left=�6+   $��127,107,0,0q�6F   $	��Top=�6[   $ -3.74,-2.112&�6s   �� 5   
   $��Soruce Field Name��6   $
��Left=�6,   $��133,222,0,0q�6G   $	��Top=�6\   $ -3.74,-2.112&�6t   $  Xceed.Wpf.Toolkit.CheckListBox  5   
   -   5          Name$ chkListBoxSourceWorksheet��5      $
��Left=�6?   $��80��6Z   $��
77,134,0,0q�6f   $	��Top=�6z   $	��205��6�   6�     5   
   -   5        $ chkListBoxSourceFieldName��5      $
��Left=�6?   $	��303��6Z   $��
77,249,0,0q�6g   $	��Top=�6{   $	��205��6�   6�   �� 5   
   $��RLD file Path :��6   $
��Left=�6*   $��	38,55,0,0q�6E   $	��Top=�6X   $ -3.74,-2.112&�6p   �� 5   
   -   5        $ txtRLDFileLocation��5      $
��Left=�6.   $��23��6I   $��
122,58,0,0q�6U   $
 Wrap=�6i   $�� ��6}   $	��Top=�6�   $	��551��6�   �� 5   
   -   5        $ btnBrowseRLDFile��5      $��Browse��6+   $
��Left=�6<   $��
678,58,0,0q�6W   $	��Top=�6k   $��75��6�   $ 8.079,-5.321&�6�   6�   �� 5   
   $��Target WrokSheet��6   $
��Left=�6+   $��355,107,0,0q�6F   $	��Top=�6[   $ -3.74,-2.112&�6s   �� 5   
   $��Target Field Name��6   $
��Left=�6,   $��361,222,0,0q�6G   $	��Top=�6\   $ -3.74,-2.112&�6t     5   
   -   5        $ chkListBoxTargetWorksheet��5      $
��Left=�6?   $��80��6Z   $��305,134,0,0q�6f   $	��Top=�6{   $	��205��6�   6�     5   
   -   5        $ chkListBoxTargetFieldName��5      $
��Left=�6?   $	��303��6Z   $��305,249,0,0q�6g   $	��Top=�6|   $	��205��6�   6�   �� 5   
   -	   5        $ 
btnExecute��5      $��Execute��6%   $
��Left=�67   $��608,133,0,0q�6R   $	��Top=�6g   $	��108��6   $��31��6�   6�   �� 5   
   -
   5        $ btnClose��5      $��Close��6#   $
��Left=�63   $��624,180,0,0q�6N   $	��Top=�6c   $��75��6{   $ 8.079,-5.321&�6�   6�   �� 5   
   $�� ��6   $
��Left=�6   $��556,249,0,0q�68   $	��Top=�6M   $��78��6e   $	��226��6q   .��5      � .+�5      �� -   5         ��Name$ 	lblStatus��5      $��Status��6+   $
��Left=�6<   $��	10,25,0,0q�6W   $	��Top=�6j   $��25��6�   $	��263��6�   �� 5      -   5        $ lblProgress��5      $��Progress : ��6-   $
��Left=�6C   $��10,0,0,0q�6^   $	��Top=�6p   $��25��6�   $	��263��6�   5      5      �� 5   
   -   5        $ btnKillProcess��5      $��Kill Process��6)   $
��Left=�6@   $��608,422,0,0q�6[   $	��Top=�6p   $	��108��6�   $��31��6�   6�   5       5!      