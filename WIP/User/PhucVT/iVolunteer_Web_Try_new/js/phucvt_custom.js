$(document).ready(function(){
    $('[data-toggle="tooltip"]').tooltip();
});

function registerformchange(){
    $("#register-form-1").css({display:"none" })
    $("#register-form-2").css({display:"block"});
}
function registerformback(){
    $("#register-form-1").css({display:"block" })
    $("#register-form-2").css({display:"none"});
}

