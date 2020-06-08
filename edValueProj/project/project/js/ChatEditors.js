document.title = "צ'ט עורכי תוכן";
var userOBJ;
var subj;
var editors;

subDict = {
    "היסטוריה": "History",
    "תנך": "Bible",
    "מדעים":"Science"
},

$(document).ready(function () {
	$('#action_menu_btn').click(function () {
		$('.action_menu').toggle();
    });

  

    if (localStorage["user"]) {
        userOBJ = JSON.parse(localStorage["user"]);
    }

    if (localStorage['quizSubject']) {
        subj = localStorage['quizSubject'];
        $(".img_cont>img").prop("src", `../images/${subDict[subj]}.jpg`);
        $(".user_info>span").html(subj);
        document.title =`צ'ט עורכי תוכן-${subj}`;
    }
    ajaxCall("PUT", "../api/Teacher/getEditors", JSON.stringify(userOBJ), getEditorsSuccess, getEditorsError);


    $(".send_btn").click(function () {
        const timeForDate = { hour: 'numeric', minute: 'numeric' };
        message = $(".type_msg").val();
        if ($.trim(message) === '') {
            return false;
        }
        

        firebase.database().ref(`/${subDict[subj]}`).push().set({
            Fullname: userOBJ.Name + " " + userOBJ.LastName,
            userMail: userOBJ.Mail.replace(".", "_").replace("@", "~"),
            message: message,
            date:new Date().getTime()

        }),

        $(`<div class="d-flex justify-content-start mb-4">
                            <div class="img_cont_msg">
                                <img src="../images/assistant.png" class="rounded-circle user_img_msg">
                            </div>
                            <div class="msg_cotainer">
                            <span title='${userOBJ.Mail}'>${userOBJ.Name} ${userOBJ.LastName}:</span>
                               ${message}
                                <span class="msg_time">${new Date().toLocaleDateString(undefined, timeForDate)}</span>
                                

                            </div>
                        </div>`).appendTo($('.msg_card_body'));

        $('.type_msg').val(null);
        $('.contact.active .preview').html('<span>You: </span>' + message);
        $(".msg_card_body").animate({ scrollTop: $(".msg_card_body")[0].scrollHeight }, 1000);
      
    });



});

function getColorbyUser(userMail) {
    for (x in editors) {

        if (editors[x].Mail === userMail) {
            return editors[x].randomColor;
        }

    }
}

function getRandomColor() {

    return "hsl(" + 360 * Math.random() + ',' +
        (90 + 10 * Math.random()) + '%,' +
        (45 +5 * Math.random()) + '%)';
//    var letters = '0123456789ABCDEF';
//    var color = '#';
//    for (var i = 0; i < 6; i++) {
//        color += letters[Math.floor(Math.random() * 16)];
//    }
//    return color;
}


function startSI() {
    var flag = true;
    var lastDate = new Date(0);
    firebase.database().ref(`/${subDict[subj]}`).on('child_added', function (snapshot) {
        setInterval(function () {
            const timeForDate = { hour: 'numeric', minute: 'numeric' };
            var message = snapshot.child("message").val();
            var Fullname = snapshot.child("Fullname").val();
            var date = snapshot.child("date").val();
            var userMail = snapshot.child("userMail").val().replace("_", ".").replace("~", "@");
            
            if (date > lastDate) {
                
                if (userMail !== userOBJ.Mail) {
                    color = getColorbyUser(userMail);
                    $(`	<div  class="d-flex justify-content-end mb-4">
                            <div  class="img_cont_msg">
                    <p style='background-color:${color}' class="rounded-circle user_img_msg">${Fullname.split(" ")[0][0]}.${Fullname.split(" ")[1][0]}</p>
                                
                            </div>
								<div style='background-color:${color}' class="msg_cotainer_send">
                                 <span title='${userMail}'>${Fullname}:</span>
									${message}
									<span class="msg_time_send">${new Date(date).toLocaleDateString(undefined, timeForDate)}</span>
								</div>`).appendTo($('.msg_card_body'));

                    $('.type_msg').val(null);
                    $('.contact.active .preview').html('<span>You: </span>' + message);
                    
                    //$(".msg_card_body").animate({ scrollTop: $(document).height() }, "fast");

                    


                }
                else if (userMail === userOBJ.Mail && flag) {
                    $(`<div class="d-flex justify-content-start mb-4">
                            <div class="img_cont_msg">
                                <img src="../images/assistant.png" class="rounded-circle user_img_msg">
                            </div>
                            <div class="msg_cotainer">
                            <span title='${userMail}'>${Fullname}:</span>
                               ${message}
                                <span class="msg_time">${new Date(date).toLocaleDateString(undefined, timeForDate)}</span>
                                

                            </div>
                        </div>`).appendTo($('.msg_card_body'));


                    $('.type_msg').val(null);
                    $('.contact.active .preview').html('<span>You: </span>' + message);
                    //$(".msg_card_body").animate({ scrollTop: $(document).height() }, "fast");
                    
                }
                lastDate = date;

                setTimeout(function () {
                    if (flag) {
                        $(".msg_card_body").animate({ scrollTop: $(".msg_card_body")[0].scrollHeight });
                    }

                    flag = false;
                }, 3000);
            }
        }, 3000);


    });
}


function getEditorsSuccess(data) {
    editors = data;
	console.log(data);
    tStr = "";
   
    for (t in editors) {
        rc = getRandomColor();
        editors[t].randomColor = rc;
        tStr += `<li class="">
                      <div class="d-flex bd-highlight">
                                    <div  class="img_cont">
                                        <p style='background-color:${rc}' class="rounded-circle user_img">${editors[t].Fname[0]}.${editors[t].LastName[0]}</p>
                                      
                                       
                                    </div>
                                    <div class="user_info">
                                        <span>${editors[t].Fname} ${editors[t].LastName}</span>
                                        <p>${editors[t].SchoolName}</p>
                                    </div>
                                </div>
                            </li>`;

    }
    startSI()
    $(".contacts").html(tStr);
}

function getEditorsError(err) {

}
