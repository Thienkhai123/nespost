(function ($) {
    var chatHub = $.connection.chatHub;

    function ChatNow() {
        //$('.chat-message-content').load('/MessagePage/Test');
        window.location.href = "/MessagePage/Test";

        //window.open("/MessagePage/Test");
        //$('span.chat-message-counter').html('');
        //if ($('#live-chat .chat').hasClass("d-none")) {
        //    $('.chat-close i').removeClass("fa-plus").addClass("fa-minus");
        //    var height = $('.chat-message-content')[0].scrollHeight;
        //    $('.chat-message-content').scrollTop(height);
        //    $('#live-chat .chat').removeClass("d-none");
        //}
        //else {
        //    $('.chat-close i').removeClass("fa-minus").addClass("fa-plus");
        //    $('#live-chat .chat').addClass("d-none");
        //}
    };

    function SendMess_Click() {
        var msg = $('#message').val();
        chatHub.server.sendPrivateMessage("", msg);
        $('#message').val('');
    };

    function txtMess_KeyPress(e) {
        if (e.which == 13) {
            SendMess_Click();
        }
    };

    //chatHub.client.onConnected = function (UserID, lstMess) {
    //    var data = lstMess;
    //    for (var i in data) {
    //        if (data[i].Fr_UserID !== UserID) {
    //            $('.chat-message-content').append('<div class="col-md-12"><p style="float:left"><strong>' + data[i].Fr_UserName + '</strong> <br/> ' + data[i].Message + ' </p></div>');
    //        }
    //        else {
    //            $('.chat-message-content').append('<div class="col-md-12"><p style="float:right"> ' + data[i].Message + '  </p></div>');
    //        }
    //    }
    //    var height = $('.chat-history')[0].scrollHeight;
    //    $('.chat-history').scrollTop(height);
    //    $('#live-chat .chat').removeClass("d-none");
    //  //  $('#live-chat .header').removeClass('d-none');
    //}

    chatHub.client.sendPrivateMessage = function (dtoMess, UserNameSend, NameSend) {
        if (dtoMess.Fr_UserRole == "CS" || dtoMess.Fr_UserRole == "CS_ADMIN") {
            ua = navigator.userAgent;
            /* MSIE used to detect old browsers and Trident used to newer ones*/
            var is_ie = ua.indexOf("MSIE ") > -1 || ua.indexOf("Trident/") > -1;

            if (is_ie == false) {
                //notification
                if (window.Notification && Notification.permission === "granted") {
                    var notification = new Notification(dtoMess.Fr_UserName, {
                        icon: "/lib/images/logo_LOGIN.png",
                        body: dtoMess.Message,
                    });

                    notification.onclick = function () {
                        window.focus();
                        this.close();
                    };
                    setTimeout(function () {
                        notification.close();
                    }, 5000);
                } else {
                    Notification.requestPermission();
                }
            }

            if (!$('#live-chat .chat').hasClass("d-none")) {
                $('.chat-message-content').append('<div class="col-md-12"><p style="float:left"> <strong>' + dtoMess.Fr_UserName + '</strong><br/> ' + dtoMess.Message + '</p></div>');
                chatHub.server.updateReadMess(dtoMess, true);
                var height = $('.chat-message-content')[0].scrollHeight;
                $('.chat-message-content').scrollTop(height);
            }
            else {
                $('.chat-message-content').append('<div class="col-md-12"><p style="float:left"><strong>' + dtoMess.Fr_UserName + '</strong><br/>' + dtoMess.Message + '</p></div>');
                var dem = parseInt($("span.chat-message-counter").html() == "" ? "0" : $("span.chat-message-counter").html()) + 1;
                $('span.chat-message-counter').html(dem);
                chatHub.server.updateReadMess(dtoMess, false);
            }
        }
        else if (dtoMess.Fr_UserRole == "HT") {
            if (!$('#live-chat .chat').hasClass("d-none")) {
                $('.chat-message-content').append('<div class="col-md-12"><em style="float:left">' + dtoMess.Message + '</em></div>');
                var height = $('.chat-message-content')[0].scrollHeight;
                $('.chat-message-content').scrollTop(height);
            }
            else {
                $('.chat-message-content').append('<div class="col-md-12"><em style="float:left">' + dtoMess.Message + '</em></div>');
                var dem = parseInt($("span.chat-message-counter").html() == "" ? "0" : $("span.chat-message-counter").html()) + 1;
                $('span.chat-message-counter').html(dem);
            }
        }
        else {
            $('.chat-message-content').append('<div class="col-md-12"><p style="float:right">' + dtoMess.Message + '</p></div>');
            var height = $('.chat-message-content')[0].scrollHeight;
            $('.chat-message-content').scrollTop(height);
        }
    }

    chatHub.client.sendErorr = function (title) {
        Swal.fire({
            title: title,
            icon: 'error',
            showCancelButton: false,
            cancelButtonText: 'No',
            confirmButtonText: 'OK'
        })
            .then(function (willDelete) {
                if (willDelete.value) {
                    window.location.reload();
                }
            });
    }

    window.ChatNow = ChatNow;
    window.txtMess_KeyPress = txtMess_KeyPress;
    window.SendMess_Click = SendMess_Click;
})(jQuery);
