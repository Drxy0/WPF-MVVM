   M S B A M L   `   `   `  ����c  _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  5   	   I ENetworkService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:NetworkServiceNetworkService 5   	   ?$clr-namespace:NetworkService.HelpersNetworkService.Helpers 5   	   ?$clr-namespace:NetworkService.HelpersNetworkService.Helpers 5	   	   C&clr-namespace:NetworkService.ViewModelNetworkService.ViewModel 5
   	   ;"clr-namespace:NetworkService.ViewsNetworkService.Views 5   	   W SNotification.Wpf, Version=8.0.0.0, Culture=neutral, PublicKeyToken=1acac493089262b5_Aclr-namespace:Notification.Wpf.Controls;assembly=Notification.WpfNotification.Wpf.Controls 5   	      NetworkService.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5   	   .localclr-namespace:NetworkService     5   	   8helpers$clr-namespace:NetworkService.Helpers     5   	   9terminal$clr-namespace:NetworkService.Helpers     5	   	   <	viewModel&clr-namespace:NetworkService.ViewModel     5
   	   4views"clr-namespace:NetworkService.Views     5   	   [notificationsAclr-namespace:Notification.Wpf.Controls;assembly=Notification.Wpf     5   	     � 
ResizeMode$  CanMinimize=�5   	    � Title$ 
MainWindow��5   	   $	��800��5   	   $
��1420��5   	    � DataContext 5      2  ,NetworkService.ViewModel.MainWindowViewModel  5   
    � 	Resources 5       �� DataType0  *NetworkService.ViewModel.EntitiesViewModel'  !NetworkService.Views.EntitiesView/  )NetworkService.ViewModel.DisplayViewModel&   NetworkService.Views.DisplayView-  'NetworkService.ViewModel.GraphViewModel$  NetworkService.Views.GraphView��%�   (��       5      *, +)(�� 2     5      *, +)(�� d     5      *, +)�� 5   
   "  6   .R�5        5      �� 5   
   "  6   .R�5        5      �� 5   
   "  6   .R�5        5      5        J� InputBindings 5"      �� 5#   
   
 �� Key$
 Oem3��5$       �� 	Modifiers$ Shiftf�5%       �� Command 5#      �� *ShiftBacktickCommand+5%      �� 5&   
   $ D1��5'      $ Shiftf�5(       5&      �� *Shift1_Command+5(      �� 5)   
   $ D2��5*      $ Shiftf�5+       5)      �� *Shift2_Command+5+      �� 5,   
   $ D3��5-      $ Shiftf�5.       5,      �� *Shift3_Command+5.      �� 5/   
   $ OemCloseBrackets��50      $ Shiftf�51       5/      �� *ShiftUp_Command+51      �� 52   
   $ OemPipe��53      $ Shiftf�54       52      �� *ShiftDown_Command+54      �� 55   
   $ Z��56      $
 Ctrlf�57       55      �� *CtrlZ_Command+57      �� 58   
   $ X��59      $
 Ctrlf�5:       58      �� *CtrlX_Command+5:      .��5=      � 	 � ColumnDefinitions	 5>   
   �� 5?      $	��100 �6   �� 5@      .+�5C   
   0  *Notification.Wpf.Controls.NotificationArea  -   5        
  Name$
 MainNotificationArea��5C   )   $��1��5D   )    K� ZIndex$
 1000��5E   )      MaxItems$ 2��5F   )      Position$ 	TopCenter=�5G   )   $	��300��5H   )   �� 5K   
   $	��100��6   $	��800��5L      $��5��5M      $
��Left=�5N      
�������5O      �� 5R   
   -   5         ��Name$ 
UndoButton��5R      $	��Top=�5S       � ToolTip$
 Undo��5T       � Cursor$
 Hand��5U      $��0,20,0,0q�5V         SidebarButtonStyle#��[  5X      ��5W      �� *UndoCommand+��5Y      �� 5Z       �� ImageSource$ Assets/arrow-undo.png��6    j� Stretch$ Uniform=�5[      5]      �� 5_   
   -   5        $ PreviousViewButton��5_      $	��Top=�5`      $
 Undo��5a      $
 Hand��5b      $��	0,100,0,0q�5c      $��80��5d      $��70��5e      #��[  5g      ��5f      �� *PreviousViewCommand+��5h      �� 5i      $ Assets/previous_view.png��6   $ Uniform=�5j      5l      �� 5o   
   -   5        $ EntitiesViewButton��5o      $	��Top=�5p      $��	0,240,0,0q�5q      $ Network Entities View��5r      $
 Hand��5s      $��entities��5u      #��[  5v      ��5t      �� *
NavCommand+��5w      �� 5x      $ Uniform=�5y       5x      �� *EntitiesIcon+5y      5{      �� 5}   
   -   5        $ DisplayViewButton��5}      $	��Top=�5~      $��	0,350,0,0q�5      $ Network Display View��5�      $
 Hand��5�      $��display��5�      #��[  5�      ��5�      �� *
NavCommand+��5�      �� 5�      $ Uniform=�5�       5�      �� *DisplayIcon+5�      5�      �� 5�   
   -   5        $ GraphViewButton��5�      $	��Top=�5�      $ Measurment Graph View��5�      $��	0,460,0,0q�5�      $
 Hand��5�      $��graph��5�      #��[  5�      ��5�      �� *
NavCommand+��5�      �� 5�      $ Uniform=�5�       5�      �� *	GraphIcon+5�      5�      �� 5�   
   -   5        $ TerminalButton��5�      $��Bottom=�5�      $ Show/Hide Terminal��5�      $��0,0,0,20q�5�      $
 Hand��5�      #��[  5�      ��5�      �� *TerminalCommand+��5�      �� 5�      $ Uniform=�5�       5�      �� *TerminalIcon+5�      5�      �� 5�   
   $��1��6   ��5�      �� *CurrentViewModel+�� 5�   
   -   5         ��Name$ TerminalDisplayTextBox��5�      " �� VerticalScrollBarVisibility$ Hidden=�5�      $��Bottom=�5�      $��0 0 0 30q�5�      $��1��5�       �� TextWrapping$
 Wrap=�5�       �� 
IsReadOnly . 
���   �
��� � �$��Consolas)�5�      $��14(�5�      ��5�      �� *TerminalDisplayHeight+y�5�      �� *TerminalDisplayVisible+��5�      �� *TerminalDisplay+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=��� 5�   
   -	   5        $ TerminalInputTextBox��5�      
���   �
��� � �$��1��5�      $��Consolas)�5�      $��14(�5�      $��Bottom=�5�      $��30��5�      $��	140,0,0,0q�5�      /	  )NetworkService.Helpers.CaretIndexBehavior 	  
CaretIndex 5�      �� *CaretIndexTerminal+y�5�      �� *TerminalInputVisible+��5�      �� *TerminalInput+$ TwoWay=�$ PropertyChanged=�.��5�   %   .
   (System.Windows.Interactivity.Interaction 
  	Behaviors 5�      -  'NetworkService.Helpers.KeyPressBehavior  5�         Command 6,   �� *KeyPressedCommand+.��5�   '   5�      �� 5�   
   -
   5         ��Name$ TerminalLabel��5�      $��1��5�      $��14(�5�      $��Consolas)�5�      $��30��5�      $	��140��5�      $
��Left=�5�      $��Bottom=�5�      
���   �
���������5�      �� *TerminalLabel+y�5�      �� *TerminalLabelVisible+5�      5�      