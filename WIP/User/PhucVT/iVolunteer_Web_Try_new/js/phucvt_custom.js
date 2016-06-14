$(document).ready(function(){
    $('[data-toggle="tooltip"]').tooltip();
});

$("#publictab").click(function(){
	$("#project_nav").show();
    $("#main_content").attr("class","col-md-9");
})

$("#dicusstab").click(function(){
	$("#project_nav").show();
    $("#main_content").attr("class","col-md-9");
})

$("#plantab").click(function(){
    $("#project_nav").hide();
    $("#main_content").attr("class","col-md-12");
});

$("#budgettab").click(function(){
    $("#project_nav").hide();
    $("#main_content").attr("class","col-md-12");
});

$("#gallerytab").click(function(){
    $("#project_nav").hide();
    $("#main_content").attr("class","col-md-12");
});

function registerformchange(){
    $("#register-form-1").css({display:"none" })
    $("#register-form-2").css({display:"block"});
}
function registerformback(){
    $("#register-form-1").css({display:"block" })
    $("#register-form-2").css({display:"none"});
}

