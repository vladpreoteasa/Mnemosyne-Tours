# Mnemosyne-Tours
Windows Forms app for tour company database management, with SQL backend.

In order to use, load database backup and replace all the lines found in the .cs files containing the following declaration:
   static readonly string constr = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=MnemosyneTours;Data Source=DESKTOP-FJITK79\\SQLEXPRESS"
so that the new source matches your configuration.
