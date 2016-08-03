$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip({
        'selector': '',
        'container': 'body'
    });

    $("[rel='tooltip']").tooltip({
        'selector': '',
        'container': 'body'
    });

    //Set the carousel options
    $('#quote-carousel').carousel({
        pause: true,
        interval: 4000,
    });

    // ANIMATEDLY DISPLAY THE NOTIFICATION COUNTER.
    //$('#noti_Counter')
    //    .css({
    //        opacity: 0
    //    })
    //    .text('5') // ADD DYNAMIC VALUE (YOU CAN EXTRACT DATA FROM DATABASE OR XML).
    //    .css({
    //        top: '-10px'
    //    })
    //    .animate({
    //        top: '-2px',
    //        opacity: 1
    //    }, 500);

    //$('#noti_Button').click(function () {

    //    $('#friend').hide();

    //    // CHECK IF NOTIFICATION COUNTER IS HIDDEN.
    //    if ($('#friend_Counter').is(':hidden')) {
    //        // CHANGE BACKGROUND COLOR OF THE BUTTON.
    //        $('#friend_icon').css('color', '#CFD8DC');
    //    }

    //    $('#message').hide();

    //    // CHECK IF NOTIFICATION COUNTER IS HIDDEN.
    //    if ($('#mess_Counter').is(':hidden')) {
    //        // CHANGE BACKGROUND COLOR OF THE BUTTON.
    //        $('#message_icon').css('color', '#CFD8DC');
    //    }

    //    // TOGGLE (SHOW OR HIDE) NOTIFICATION WINDOW.
    //    $('#notifications').fadeToggle('fast', 'linear', function () {
    //        if ($('#notifications').is(':hidden')) {
    //            $('#notiication_icon').css('color', '#CFD8DC');
    //        } else $('#notiication_icon').css('color', '#FFF'); // CHANGE BACKGROUND COLOR OF THE BUTTON.
    //    });

    //    $('#noti_Counter').fadeOut('slow'); // HIDE THE COUNTER.

    //    return false;
    //});


    //$('#notifications').click(function () {
    //    return false; // DO NOTHING WHEN CONTAINER IS CLICKED.
    //});

    ////Friend

    //// ANIMATEDLY DISPLAY THE NOTIFICATION COUNTER.
    //$('#friend_Counter')
    //    .css({
    //        opacity: 0
    //    })
    //    .text('10') // ADD DYNAMIC VALUE (YOU CAN EXTRACT DATA FROM DATABASE OR XML).
    //    .css({
    //        top: '-10px'
    //    })
    //    .animate({
    //        top: '-2px',
    //        opacity: 1
    //    }, 500);

    //$('#friend_Button').click(function () {

    //    $('#notifications').hide();

    //    // CHECK IF NOTIFICATION COUNTER IS HIDDEN.
    //    if ($('#noti_Counter').is(':hidden')) {
    //        // CHANGE BACKGROUND COLOR OF THE BUTTON.
    //        $('#notiication_icon').css('color', '#CFD8DC');
    //    }

    //    $('#message').hide();

    //    // CHECK IF NOTIFICATION COUNTER IS HIDDEN.
    //    if ($('#mess_Counter').is(':hidden')) {
    //        // CHANGE BACKGROUND COLOR OF THE BUTTON.
    //        $('#message_icon').css('color', '#CFD8DC');
    //    }

    //    // TOGGLE (SHOW OR HIDE) NOTIFICATION WINDOW.
    //    $('#friend').fadeToggle('fast', 'linear', function () {
    //        if ($('#friend').is(':hidden')) {
    //            $('#friend_icon').css('color', '#CFD8DC');
    //        } else $('#friend_icon').css('color', '#FFF'); // CHANGE BACKGROUND COLOR OF THE BUTTON.
    //    });

    //    $('#friend_Counter').fadeOut('slow'); // HIDE THE COUNTER.

    //    return false;
    //});

    //$('#friend').click(function () {
    //    return false; // DO NOTHING WHEN CONTAINER IS CLICKED.
    //});


    ////Message

    //// ANIMATEDLY DISPLAY THE NOTIFICATION COUNTER.
    //$('#mess_Counter')
    //    .css({
    //        opacity: 0
    //    })
    //    .text('2') // ADD DYNAMIC VALUE (YOU CAN EXTRACT DATA FROM DATABASE OR XML).
    //    .css({
    //        top: '-10px'
    //    })
    //    .animate({
    //        top: '-2px',
    //        opacity: 1
    //    }, 500);

    //$('#mess_Button').click(function () {

    //    $('#notifications').hide();

    //    // CHECK IF NOTIFICATION COUNTER IS HIDDEN.
    //    if ($('#noti_Counter').is(':hidden')) {
    //        // CHANGE BACKGROUND COLOR OF THE BUTTON.
    //        $('#notiication_icon').css('color', '#CFD8DC');
    //    }

    //    $('#friend').hide();

    //    // CHECK IF NOTIFICATION COUNTER IS HIDDEN.
    //    if ($('#friend_Counter').is(':hidden')) {
    //        // CHANGE BACKGROUND COLOR OF THE BUTTON.
    //        $('#friend_icon').css('color', '#CFD8DC');
    //    }


    //    // TOGGLE (SHOW OR HIDE) NOTIFICATION WINDOW.
    //    $('#message').fadeToggle('fast', 'linear', function () {
    //        if ($('#message').is(':hidden')) {
    //            $('#message_icon').css('color', '#CFD8DC');
    //        } else $('#message_icon').css('color', '#FFF'); // CHANGE BACKGROUND COLOR OF THE BUTTON.
    //    });

    //    $('#mess_Counter').fadeOut('slow'); // HIDE THE COUNTER.

    //    return false;
    //});

    //$('#notifications').click(function () {
    //    return false; // DO NOTHING WHEN CONTAINER IS CLICKED.
    //});

    //// HIDE NOTIFICATIONS WHEN CLICKED ANYWHERE ON THE PAGE.
    //$(document).click(function () {
    //    $('#notifications').hide();

    //    // CHECK IF NOTIFICATION COUNTER IS HIDDEN.
    //    if ($('#noti_Counter').is(':hidden')) {
    //        // CHANGE BACKGROUND COLOR OF THE BUTTON.
    //        $('#notiication_icon').css('color', '#CFD8DC');
    //    }

    //    $('#friend').hide();

    //    // CHECK IF NOTIFICATION COUNTER IS HIDDEN.
    //    if ($('#friend_Counter').is(':hidden')) {
    //        // CHANGE BACKGROUND COLOR OF THE BUTTON.
    //        $('#friend_icon').css('color', '#CFD8DC');
    //    }

    //    $('#message').hide();

    //    // CHECK IF NOTIFICATION COUNTER IS HIDDEN.
    //    if ($('#mess_Counter').is(':hidden')) {
    //        // CHANGE BACKGROUND COLOR OF THE BUTTON.
    //        $('#message_icon').css('color', '#CFD8DC');
    //    }
    //});


    //test change plan
    $('#testplan').click(function () {
        $('#testplantarget').empty();
        $('#testplantarget').append('<div class="row"><!-- Header --><div class="col-md-12"><label>Lên nội dung và phương thức tuyển thành viên cho dự án</label><div class="divider-holizon"></div></div><!-- Detail --><div class="col-sm-12"><ol class="list-task"><li><div class="row"><div class="col-sm-12"><div class="priolity low"></div></div><div class="col-sm-12"><div class="checkbox"><input id="task1" type="checkbox"><label for="task1">Khởi động truyền thông và kick off dự án qua tất cả các kênh.</label></div></div><div class="col-sm-12"><ul class="list-task-infor"><li><i class="material-icons md-18 md-myred">access_time</i><span class="overdue">22:00</span></li><li><i class="material-icons md-18 md-mygreen">perm_identity</i><span class="pending">Nguyễn Trường Giang</span></li></ul></div></div></li><li><div class="row"><div class="col-sm-12"><div class="priolity high"></div></div><div class="col-sm-12"><div class="checkbox"><input id="task2" type="checkbox"><label for="task2">Lên kế hoạch chuẩn bị hậu cần.</label></div></div><div class="col-sm-12"><ul class="list-task-infor"><li><i class="material-icons md-18 md-mygreen">access_time</i><span class="pending">22:00</span></li><li><i class="material-icons md-18 md-mygreen">perm_identity</i><span class="pending">Nguyễn Trường Giang</span></li></ul></div></div></li><li><div class="row"><div class="col-sm-12"><label style="font-weight: bold; color: #049f88;">THÊM CÔNG VIỆC ...</label></div></div></li></ol></div></div>');
    });

    //edit info
    $('#editname').click(function () {
        $('#editname').hide();
        $('#savename').show();
        $('#namedisplay').hide();
        $('#txtname').show();
    });

    $('#savename').click(function () {
        $('#editname').show();
        $('#savename').hide();
        $('#namedisplay').show();
        $('#txtname').hide();
        var newname = "";
        newname = $('#txtname').val();
        $('#namedisplay').text("" + newname)
    });

    //open/ close advance search panel
    $('#advance-search').click(function () {
        $('#advance-search-panel').show(500);
        $('#advance-search').hide();
        $('#advance-search-close').show();
    });
    $('#advance-search-close').click(function () {
        $('#advance-search-panel').hide(500);
        $('#advance-search').show();
        $('#advance-search-close').hide();
    });

    // add item budget
    $('#add-budget-item-button').click(function () {
        $('#add-budget-item').show(500);
    });

    $('#save-budget-item').click(function () {
        var content = $('#budget-content').val();
        var unitprice = $('#budget-unit-price').val();
        var quatity = $('#budget-quatity').val();
        var unit = $('#budget-unit').val();
        var cost = parseFloat(unitprice) * parseInt(quatity);

        $(headman1).find('tbody').append("<tr><td>5</td><td>" + content + "</td><td>" + unitprice + "</td><td>" + quatity + "</td><td>" + unit + "</td><td>" + cost + ".000</td><td><a>SỬA</a> | <a>XÓA</a></td></tr>");
        $('#add-budget-item').hide(500);
    });

    // show name of button view access
    $('.btn-view-action').hover(function () {
        $('.btn-view-action').find('span').show(1000);
    })

});

function registerformchange() {
    $("#register-form-1").css({
        display: "none"
    })
    $("#register-form-2").css({
        display: "block"
    });
}

function registerformback() {
    $("#register-form-1").css({
        display: "block"
    })
    $("#register-form-2").css({
        display: "none"
    });
}