function disconnect() {

    localStorage.clear();
    location.replace("LogIn.html");
}

function closeCPdiv() {
    $("#disbackg").css("visibility", "hidden");
    $("body>#changePass").remove();
}

function displayCP() {
    str = `<div id="changePass">
        <form id="cpForm">
          <div onclick="closeCPdiv()" id ="closePass">X</div>
        
        <div><h3>החלף סיסמא</h3></div>
        <div id="myPass">
            <label>הכנס סיסמא נוכחית</label>
            <input  type="password" required/>
        </div>
        <div id="newPass">
            <div id="ft">
                <label>הכנס סיסמא חדשה</label>
                <input minLength='9' maxLength='18' title='הסיסמא חייבת להיות בין9 ל18 תווים' type="password" required/>
            </div>
            <div id="st">
                <label>הכנס סיסמא חדשה</label>
                <input minLength='9' maxLength='18' type="password" required/>
            </div>
           <div id=cpBtn>
            <input type='submit'>
           </div>

          </form>
        </div>

    </div>`;
    $("#disbackg").css("visibility", "visible");
    $("body").append(str);
    conectcpForm();
}

function conectcpForm(){
    $("#cpForm").submit(function (e) {
        alert();
        val = localStorage['userMode'];
        checkCP(val);
        return false;

    });
}

function changePassSuccess(data) {
    if (data === 0) {
        swal("הסיסמא הנוכחית שהזנת שגויה");
    } else {
        swal("סיסמא עודכנה בהצלחה");
        closeCPdiv();

    }
}

function changePassError(err) {

}

function checkCP(val) {

    prevPass = $("#myPass>input[type='password']").val();
    newPass1 = $("#newPass #ft>input[type='password']").val();
    newPass2 = $("#newPass #st>input[type='password']").val();
    if (newPass1 === newPass2 && prevPass !== newPass1) {

        obj = {
            newPass: newPass1,
            prevPass: prevPass,
            type: val
        };
        if (localStorage["user"])
        userOBJ = JSON.parse(localStorage["user"]);
        if (val === "Admin") {
           
           obj.userName = userOBJ.Name;
        } else {
            obj.userName = userOBJ.Mail;
        }
        ajaxCall("PUT","../api/EDvalue/updatePass", JSON.stringify(obj),changePassSuccess,changePassError);   
        
    } else {
        if (newPass1 !== newPass2) {
            swal("הסיסמאות החדשות לא תואמות");
            
        } else if (prevPass === newPass1) {
            swal('הסיסמא החדשה צריכה להיות שונה מהסיסמא הקודמת');
            
        }
    }
    
    return false;

}

