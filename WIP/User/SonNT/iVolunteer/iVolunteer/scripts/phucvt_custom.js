$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
    $("[rel='tooltip']").tooltip();

    // ANIMATEDLY DISPLAY THE NOTIFICATION COUNTER.
    $('#noti_Counter')
        .css({ opacity: 0 })
        .text('5')              // ADD DYNAMIC VALUE (YOU CAN EXTRACT DATA FROM DATABASE OR XML).
        .css({ top: '-10px' })
        .animate({ top: '-2px', opacity: 1 }, 500);

    $('#noti_Button').click(function () {

        $('#friend').hide();

        // CHECK IF NOTIFICATION COUNTER IS HIDDEN.
        if ($('#friend_Counter').is(':hidden')) {
            // CHANGE BACKGROUND COLOR OF THE BUTTON.
            $('#friend_icon').css('color', '#CFD8DC');
        }

        $('#message').hide();

        // CHECK IF NOTIFICATION COUNTER IS HIDDEN.
        if ($('#mess_Counter').is(':hidden')) {
            // CHANGE BACKGROUND COLOR OF THE BUTTON.
            $('#message_icon').css('color', '#CFD8DC');
        }

        // TOGGLE (SHOW OR HIDE) NOTIFICATION WINDOW.
        $('#notifications').fadeToggle('fast', 'linear', function () {
            if ($('#notifications').is(':hidden')) {
                $('#notiication_icon').css('color', '#CFD8DC');
            }
            else $('#notiication_icon').css('color', '#FFF');        // CHANGE BACKGROUND COLOR OF THE BUTTON.
        });

        $('#noti_Counter').fadeOut('slow');                 // HIDE THE COUNTER.

        return false;
    });


    $('#notifications').click(function () {
        return false;       // DO NOTHING WHEN CONTAINER IS CLICKED.
    });

    //Friend

    // ANIMATEDLY DISPLAY THE NOTIFICATION COUNTER.
    $('#friend_Counter')
        .css({ opacity: 0 })
        .text('10')              // ADD DYNAMIC VALUE (YOU CAN EXTRACT DATA FROM DATABASE OR XML).
        .css({ top: '-10px' })
        .animate({ top: '-2px', opacity: 1 }, 500);

    $('#friend_Button').click(function () {

        $('#notifications').hide();

        // CHECK IF NOTIFICATION COUNTER IS HIDDEN.
        if ($('#noti_Counter').is(':hidden')) {
            // CHANGE BACKGROUND COLOR OF THE BUTTON.
            $('#notiication_icon').css('color', '#CFD8DC');
        }

        $('#message').hide();

        // CHECK IF NOTIFICATION COUNTER IS HIDDEN.
        if ($('#mess_Counter').is(':hidden')) {
            // CHANGE BACKGROUND COLOR OF THE BUTTON.
            $('#message_icon').css('color', '#CFD8DC');
        }

        // TOGGLE (SHOW OR HIDE) NOTIFICATION WINDOW.
        $('#friend').fadeToggle('fast', 'linear', function () {
            if ($('#friend').is(':hidden')) {
                $('#friend_icon').css('color', '#CFD8DC');
            }
            else $('#friend_icon').css('color', '#FFF');        // CHANGE BACKGROUND COLOR OF THE BUTTON.
        });

        $('#friend_Counter').fadeOut('slow');                 // HIDE THE COUNTER.

        return false;
    });

    $('#friend').click(function () {
        return false;       // DO NOTHING WHEN CONTAINER IS CLICKED.
    });


    //Message

    // ANIMATEDLY DISPLAY THE NOTIFICATION COUNTER.
    $('#mess_Counter')
        .css({ opacity: 0 })
        .text('2')              // ADD DYNAMIC VALUE (YOU CAN EXTRACT DATA FROM DATABASE OR XML).
        .css({ top: '-10px' })
        .animate({ top: '-2px', opacity: 1 }, 500);

    $('#mess_Button').click(function () {

        $('#notifications').hide();

        // CHECK IF NOTIFICATION COUNTER IS HIDDEN.
        if ($('#noti_Counter').is(':hidden')) {
            // CHANGE BACKGROUND COLOR OF THE BUTTON.
            $('#notiication_icon').css('color', '#CFD8DC');
        }

        $('#friend').hide();

        // CHECK IF NOTIFICATION COUNTER IS HIDDEN.
        if ($('#friend_Counter').is(':hidden')) {
            // CHANGE BACKGROUND COLOR OF THE BUTTON.
            $('#friend_icon').css('color', '#CFD8DC');
        }


        // TOGGLE (SHOW OR HIDE) NOTIFICATION WINDOW.
        $('#message').fadeToggle('fast', 'linear', function () {
            if ($('#message').is(':hidden')) {
                $('#message_icon').css('color', '#CFD8DC');
            }
            else $('#message_icon').css('color', '#FFF');        // CHANGE BACKGROUND COLOR OF THE BUTTON.
        });

        $('#mess_Counter').fadeOut('slow');                 // HIDE THE COUNTER.

        return false;
    });

    $('#notifications').click(function () {
        return false;       // DO NOTHING WHEN CONTAINER IS CLICKED.
    });

    // HIDE NOTIFICATIONS WHEN CLICKED ANYWHERE ON THE PAGE.
    $(document).click(function () {
        $('#notifications').hide();

        // CHECK IF NOTIFICATION COUNTER IS HIDDEN.
        if ($('#noti_Counter').is(':hidden')) {
            // CHANGE BACKGROUND COLOR OF THE BUTTON.
            $('#notiication_icon').css('color', '#CFD8DC');
        }

        $('#friend').hide();

        // CHECK IF NOTIFICATION COUNTER IS HIDDEN.
        if ($('#friend_Counter').is(':hidden')) {
            // CHANGE BACKGROUND COLOR OF THE BUTTON.
            $('#friend_icon').css('color', '#CFD8DC');
        }

        $('#message').hide();

        // CHECK IF NOTIFICATION COUNTER IS HIDDEN.
        if ($('#mess_Counter').is(':hidden')) {
            // CHANGE BACKGROUND COLOR OF THE BUTTON.
            $('#message_icon').css('color', '#CFD8DC');
        }
    });
 
});

function registerformchange() {
    $("#register-form-1").css({ display: "none" })
    $("#register-form-2").css({ display: "block" });
}
function registerformback() {
    $("#register-form-1").css({ display: "block" })
    $("#register-form-2").css({ display: "none" });
}

