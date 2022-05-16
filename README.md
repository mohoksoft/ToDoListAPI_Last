# ToDoListAPI_Last
Instructions for using the source code:

When starting the application, a browser with the local address https: // localhost:44344 will open, so you need to add /Swagger, 
to open the Swagger interface to test all functions. A list of all controllers will be displayed on the screen, and with them all controller methods. 
The format for entering the date is: yyyy-mm-dd All fields are required.

1. Register as a new user first.
2. Enter a new list
3. Enter a new task
You can list all tasks and lists via the POST method api/list/listall and api/task/listall

When using the listall method, you can specify which page you want and the filters.

* For mailing tasks it need to change file Web.config
*  <system.net>  
*    <mailSettings>  
*        <smtp from="ToDo@gmail.com">  
*            <network host="smtp.gmail.com" port="587" userName="your_mail" password="password" enableSsl="true" />  
*        </smtp>  
*    </mailSettings>  
*</system.net> 

In yor mail and password tipe in real mail and password for sending mails to users. In ToDo@gmail.com also.
