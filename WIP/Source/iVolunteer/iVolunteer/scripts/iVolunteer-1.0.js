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

    // show name of button view access
    $('.btn-view-action').hover(function () {
        $('.btn-view-action').find('span').show(1000);
    })

});

function playSound() {
    document.getElementById("sound").innerHTML = '<audio autoplay="autoplay"><source src="/Sound/notisound.ogg" type="audio/ogg" /><embed hidden="true" autostart="true" loop="false" src="/Sound/notisound.ogg" /></audio>';
    console.log("play sound");
}

/******************************************************** Script for Loading *********************************************************/

//Show Symbol loading before ajax loading
function OnBegin(id) {
    var loadingHtml = "";
    loadingHtml += '<div class="card-panel" style="margin: 0.5rem 0 1rem 0;">';
    loadingHtml += '<div class="progress"><div class="indeterminate"></div></div>';
    loadingHtml += '</div';
    $(id).html(loadingHtml);
}

//On begin popup
function OnBeginPopup(idloading, idopen) {
    var loadingPopup = "";
    loadingPopup += '<div class="card-panel" style="margin: 0.5rem 0 1rem 0; text-align: center; height: 120px; margin-top: 30px;">';
    loadingPopup += '<div class="preloader-wrapper active">';
    loadingPopup += '<div class="spinner-layer spinner-mygreen-only">';
    loadingPopup += '<div class="circle-clipper left">';
    loadingPopup += '<div class="circle"></div>';
    loadingPopup += '</div><div class="gap-patch">';
    loadingPopup += '<div class="circle"></div>';
    loadingPopup += '</div><div class="circle-clipper right">';
    loadingPopup += '<div class="circle"></div>';
    loadingPopup += '</div>';
    loadingPopup += '</div>';
    loadingPopup += '</div>';
    loadingPopup += '<p class="loading">Loading<span>.</span><span>.</span><span>.</span></p>'
    loadingPopup += '</div>';
    $(idloading).html(loadingPopup);
    $(idopen).modal('show');
}

//On begin noti_Buttonfunction 
function OnBeginNoti() {
    var loadingHtml = '<div class="progress2"><div class="indeterminate"></div></div>';
    $("#notifyPage").html(loadingHtml);
};

/****************************************************************  Script for Search Bar ***********************************************/
$("#searchArea").keyup(function () {
    delay(function () {
        $("#srchGrVal").html($("#searchArea").val());
        $("#srchPrVal").html($("#searchArea").val());
        $("#srchUsVal").html($("#searchArea").val());

        var searchAreaWidth = $('#searchArea').width() + 35;
        $('#listSearchBar').width(searchAreaWidth);

        var searchItemWidth = $('#searchArea').width() - 10;
        $("#srchGrVal").css('max-width', searchItemWidth + 'px');
        $("#srchPrVal").css('max-width', searchItemWidth + 'px');
        $("#srchUsVal").css('max-width', searchItemWidth + 'px');

        $(".dropdown-content").show();

    }, 100);

});

$('div.search-dropdown').mouseleave(function () {
    $(".dropdown-content").hide();
});

if ($("#searchArea").val().length < 1) {
    $(".dropdown-content").hide();
}
var delay = (function () {
    var timer = 0;
    return function (callback, ms) {
        clearTimeout(timer);
        timer = setTimeout(callback, ms);
    };
})();

$(document).click(function (event) {
    if (!$(event.target).closest('.dropdown-content').length) {
        if ($('.dropdown-content').is(":visible")) {
            $('.dropdown-content').hide();
        }
    }
});
var $listItems = $('li.searchItem');

function search(selector) {
    var name = $("#searchArea").val();
    var option = $(selector).attr('role');
    window.location.href = "/Home/Search?name=" + name + "&option=" + option;
}
var $current;
$('#searchArea').keydown(function (e) {
    var key = e.keyCode,
        $selected = $listItems.filter('.selected');


    if (key != 40 && key != 38 && key != 13) return;

    $listItems.removeClass('selected');

    if (key == 40) // Down key
    {
        if (!$selected.length || $selected.is(':last-child')) {
            $current = $listItems.eq(0);
        } else {
            $current = $selected.next();
        }
    } else if (key == 38) // Up key
    {
        if (!$selected.length || $selected.is(':first-child')) {
            $current = $listItems.last();
        } else {
            $current = $selected.prev();
        }
    } else if (key == 13) {
        search($current);
    }
    $current.addClass('selected');
});

/************************************************************ Show Snack bar ***************************************************/
function showSnackBar() {
    var x = document.getElementById("snackbar")
    x.className = "snackbar show";
    setTimeout(function () { x.className = x.className.replace("show", ""); }, 3000);
}
function deleteSuccess() {
    $("#snackbar").html("Xóa thành công.");
    showSnackBar();
}
function editSuccess() {
    $("#snackbar").html("Sửa thành công.");
    showSnackBar();
}
function addSuccess() {
    $("#snackbar").html("Thêm thành công.");
    showSnackBar();
}
function acceptSuccess() {
    $("#snackbar").html("Đã đồng ý.");
    showSnackBar();
}
function declineSuccess() {
    $("#snackbar").html("Đã từ chối.");
    showSnackBar();
}
function changeSuccess() {
    $("#snackbar").html("Thay đổi thành công.");
    showSnackBar();
}

/* ****************************** FANCY BOX ************************************ */
$(document).ready(function () {
    /*
     *  Simple image gallery. Uses default settings
     */

    $('.fancybox').fancybox({
        padding: 0,
        openEffect: 'elastic',
        openSpeed: 150,

        closeEffect: 'elastic',
        closeSpeed: 150,
    });

    /*
     *  Different effects
     */
});

/************************************ Reload Page *********************************************/
function reloadCurrentPage() {
    location.reload();
    changeSuccess();
}

function onpopupclose(id) {
    $(id).modal('toggle');
}

function closeDialog(id) {
    $(id).dialog('close');
}
/*********************************** Clear Form *************************************************/
function clearForm(id) {
    $(id).empty();
};

/*********************************** Show Image Album Justify *************************************/
$('#links').justifiedGallery({
    rowHeight: 200,
    lastRow: 'nojustify',
    randomize: true,
    margins: 10
});

/********************************** Replace Emoticon **************************************/
function replaceEmoji(message) {
    message = message.replace(/:\)\)/g, "<img src=\"/Images/Emoji/emoji-1.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-2.png\" class=\"smileys\" />");
    message = message.replace(/:\)/g, "<img src=\"/Images/Emoji/emoji-3.png\" class=\"smileys\" />");
    message = message.replace(/=\)/g, "<img src=\"/Images/Emoji/emoji-4.png\" class=\"smileys\" />");
    message = message.replace(/.-\)/g, "<img src=\"/Images/Emoji/emoji-5.png\" class=\"smileys\" />");
    message = message.replace(/<3<3/g, "<img src=\"/Images/Emoji/emoji-6.png\" class=\"smileys\" />");
    message = message.replace(/:x/g, "<img src=\"/Images/Emoji/emoji-7.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-8.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-9.png\" class=\"smileys\" />");
    message = message.replace(/><p/g, "<img src=\"/Images/Emoji/emoji-10.png\" class=\"smileys\" />");
    message = message.replace(/_op/g, "<img src=\"/Images/Emoji/emoji-11.png\" class=\"smileys\" />");
    message = message.replace(/><p/g, "<img src=\"/Images/Emoji/emoji-12.png\" class=\"smileys\" />");
    message = message.replace(/:punch:/g, "<img src=\"/Images/Emoji/emoji-108.png\" class=\"smileys\" />");
    message = message.replace(/:poop:/g, "<img src=\"/Images/Emoji/emoji-89.png\" class=\"smileys\" />");
    message = message.replace(/:p/g, "<img src=\"/Images/Emoji/emoji-13.png\" class=\"smileys\" />");
    message = message.replace(/:o/g, "<img src=\"/Images/Emoji/emoji-14.png\" class=\"smileys\" />");
    message = message.replace(/:h/g, "<img src=\"/Images/Emoji/emoji-15.png\" class=\"smileys\" />");
    message = message.replace(/><\(/g, "<img src=\"/Images/Emoji/emoji-16.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-17.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-18.png\" class=\"smileys\" />");
    message = message.replace(/:\(/g, "<img src=\"/Images/Emoji/emoji-19.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-20.png\" class=\"smileys\" />");
    message = message.replace(/:\'\(/g, "<img src=\"/Images/Emoji/emoji-21.png\" class=\"smileys\" />");
    message = message.replace(/:\"\)/g, "<img src=\"/Images/Emoji/emoji-22.png\" class=\"smileys\" />");
    message = message.replace(/ToT/g, "<img src=\"/Images/Emoji/emoji-23.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-24.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-25.png\" class=\"smileys\" />");
    message = message.replace(/:'-&/g, "<img src=\"/Images/Emoji/emoji-26.png\" class=\"smileys\" />");
    message = message.replace(/:\'\)/g, "<img src=\"/Images/Emoji/emoji-27.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-28.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-29.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-30.png\" class=\"smileys\" />");
    message = message.replace(/:-&/g, "<img src=\"/Images/Emoji/emoji-31.png\" class=\"smileys\" />");
    message = message.replace(/\\OoO\//g, "<img src=\"/Images/Emoji/emoji-32.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-33.png\" class=\"smileys\" />");
    message = message.replace(/X\(/g, "<img src=\"/Images/Emoji/emoji-34.png\" class=\"smileys\" />");
    message = message.replace(/:2/g, "<img src=\"/Images/Emoji/emoji-35.png\" class=\"smileys\" />");
    message = message.replace(/><w/g, "<img src=\"/Images/Emoji/emoji-36.png\" class=\"smileys\" />");
    message = message.replace(/><\)\)/g, "<img src=\"/Images/Emoji/emoji-37.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-38.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-39.png\" class=\"smileys\" />");
    message = message.replace(/B-\)/g, "<img src=\"/Images/Emoji/emoji-40.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-41.png\" class=\"smileys\" />");
    message = message.replace(/xOx/g, "<img src=\"/Images/Emoji/emoji-42.png\" class=\"smileys\" />");
    message = message.replace(/xox/g, "<img src=\"/Images/Emoji/emoji-43.png\" class=\"smileys\" />");
    message = message.replace(/:zzz/g, "<img src=\"/Images/Emoji/emoji-44.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-45.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-46.png\" class=\"smileys\" />");
    message = message.replace(/>:\)/g, "<img src=\"/Images/Emoji/emoji-47.png\" class=\"smileys\" />");
    message = message.replace(/>:\(/g, "<img src=\"/Images/Emoji/emoji-48.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-49.png\" class=\"smileys\" />");
    message = message.replace(/:D/g, "<img src=\"/Images/Emoji/emoji-50.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-51.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-52.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-53.png\" class=\"smileys\" />");
    message = message.replace(/O:/g, "<img src=\"/Images/Emoji/emoji-54.png\" class=\"smileys\" />");
    message = message.replace(/O:-\)/g, "<img src=\"/Images/Emoji/emoji-55.png\" class=\"smileys\" />");
    message = message.replace(/--l/g, "<img src=\"/Images/Emoji/emoji-56.png\" class=\"smileys\" />");
    message = message.replace(/-_-/g, "<img src=\"/Images/Emoji/emoji-57.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-58.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-59.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-60.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-61.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-62.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-63.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-64.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-65.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-66.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-67.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-68.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-69.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-70.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-71.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-72.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-73.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-74.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-75.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-76.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-77.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-78.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-79.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-80.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-81.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-82.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-83.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-84.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-85.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-86.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-87.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-88.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-90.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-91.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-92.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-93.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-94.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-95.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-96.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-97.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-98.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-99.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-100.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-101.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-102.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-103.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-104.png\" class=\"smileys\" />");
    message = message.replace(/\(y\)/g, "<img src=\"/Images/Emoji/emoji-105.png\" class=\"smileys\" />");
    message = message.replace(/\(n\)/g, "<img src=\"/Images/Emoji/emoji-106.png\" class=\"smileys\" />");
    message = message.replace(/:ok:/g, "<img src=\"/Images/Emoji/emoji-107.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-109.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-110.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-111.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-112.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-113.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-114.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-115.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-116.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-117.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-118.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-119.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-120.png\" class=\"smileys\" />");
    message = message.replace(/:clap:/g, "<img src=\"/Images/Emoji/emoji-121.png\" class=\"smileys\" />");
    message = message.replace(/:strong:/g, "<img src=\"/Images/Emoji/emoji-122.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-123.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-124.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-125.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-126.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-127.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-128.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-129.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-130.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-131.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-132.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-133.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-134.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-135.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-136.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-137.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-138.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-139.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-140.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-141.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-142.png\" class=\"smileys\" />");
    message = message.replace(/:sr:/g, "<img src=\"/Images/Emoji/emoji-143.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-144.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-145.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-146.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-147.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-148.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-149.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-150.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-151.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-152.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-153.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-154.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-155.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-156.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-157.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-158.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-159.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-160.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-161.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-162.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-163.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-164.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-165.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-166.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-167.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-168.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-169.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-170.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-171.png\" class=\"smileys\" />");
    message = message.replace(/<3/g, "<img src=\"/Images/Emoji/emoji-172.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-173.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-174.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-175.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-176.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-177.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-178.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-179.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-180.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-181.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-182.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-183.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-184.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-185.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-186.png\" class=\"smileys\" />");
    message = message.replace("abcxyzmno", "<img src=\"/Images/Emoji/emoji-187.png\" class=\"smileys\" />");
    message = message.replace(/:v/g, "<img src=\"/Images/Emoji/emoji-191.png\" class=\"smileys\" />");
    message = message.replace(/:3/g, "<img src=\"/Images/Emoji/emoji-192.png\" class=\"smileys\" />");

    return message;
}

$(function () {
    $('.table-report').each(function () {
        if ($(this).find('thead').length > 0 && $(this).find('th').length > 0) {
            // Clone <thead>
            var $w = $(window),
				$t = $(this),
				$thead = $t.find('thead').clone(),
				$col = $t.find('thead, tbody').clone();

            // Add class, remove margins, reset width and wrap table
            $t
			.addClass('sticky-enabled')
			.css({
			    margin: 0,
			    width: '100%'
			}).wrap('<div class="sticky-wrap" />');

            if ($t.hasClass('overflow-y')) $t.removeClass('overflow-y').parent().addClass('overflow-y');

            // Create new sticky table head (basic)
            $t.after('<table class="sticky-thead" />');

            // If <tbody> contains <th>, then we create sticky column and intersect (advanced)
            if ($t.find('tbody th').length > 0) {
                $t.after('<table class="sticky-col" /><table class="sticky-intersect" />');
            }

            // Create shorthand for things
            var $stickyHead = $(this).siblings('.sticky-thead'),
				$stickyCol = $(this).siblings('.sticky-col'),
				$stickyInsct = $(this).siblings('.sticky-intersect'),
				$stickyWrap = $(this).parent('.sticky-wrap');

            $stickyHead.append($thead);

            $stickyCol
			.append($col)
				.find('thead th:gt(0)').remove()
				.end()
				.find('tbody td').remove();

            $stickyInsct.html('<thead><tr><th>' + $t.find('thead th:first-child').html() + '</th></tr></thead>');

            // Set widths
            var setWidths = function () {
                $t
                .find('thead th').each(function (i) {
                    $stickyHead.find('th').eq(i).width($(this).width());
                })
                .end()
                .find('tr').each(function (i) {
                    $stickyCol.find('tr').eq(i).height($(this).height());
                });

                // Set width of sticky table head
                $stickyHead.width($t.width());

                // Set width of sticky table col
                $stickyCol.find('th').add($stickyInsct.find('th')).width($t.find('thead th').width())
            },
				repositionStickyHead = function () {
				    // Return value of calculated allowance
				    var allowance = calcAllowance();

				    // Check if wrapper parent is overflowing along the y-axis
				    if ($t.height() > $stickyWrap.height()) {
				        // If it is overflowing (advanced layout)
				        // Position sticky header based on wrapper scrollTop()
				        if ($stickyWrap.scrollTop() > 0) {
				            // When top of wrapping parent is out of view
				            $stickyHead.add($stickyInsct).css({
				                opacity: 1,
				                top: $stickyWrap.scrollTop()
				            });
				        } else {
				            // When top of wrapping parent is in view
				            $stickyHead.add($stickyInsct).css({
				                opacity: 0,
				                top: 0
				            });
				        }
				    } else {
				        // If it is not overflowing (basic layout)
				        // Position sticky header based on viewport scrollTop
				        if ($w.scrollTop() > $t.offset().top && $w.scrollTop() < $t.offset().top + $t.outerHeight() - allowance) {
				            // When top of viewport is in the table itself
				            $stickyHead.add($stickyInsct).css({
				                opacity: 1,
				                top: $w.scrollTop() - $t.offset().top
				            });
				        } else {
				            // When top of viewport is above or below table
				            $stickyHead.add($stickyInsct).css({
				                opacity: 0,
				                top: 0
				            });
				        }
				    }
				},
				repositionStickyCol = function () {
				    if ($stickyWrap.scrollLeft() > 0) {
				        // When left of wrapping parent is out of view
				        $stickyCol.add($stickyInsct).css({
				            opacity: 1,
				            left: $stickyWrap.scrollLeft()
				        });
				    } else {
				        // When left of wrapping parent is in view
				        $stickyCol
						.css({ opacity: 0 })
						.add($stickyInsct).css({ left: 0 });
				    }
				},
				calcAllowance = function () {
				    var a = 0;
				    // Calculate allowance
				    $t.find('tbody tr:lt(3)').each(function () {
				        a += $(this).height();
				    });

				    // Set fail safe limit (last three row might be too tall)
				    // Set arbitrary limit at 0.25 of viewport height, or you can use an arbitrary pixel value
				    if (a > $w.height() * 0.25) {
				        a = $w.height() * 0.25;
				    }

				    // Add the height of sticky header
				    a += $stickyHead.height();
				    return a;
				};

            setWidths();

            $t.parent('.sticky-wrap').scroll($.throttle(250, function () {
                repositionStickyHead();
                repositionStickyCol();
            }));

            $w
			.load(setWidths)
			.resize($.debounce(250, function () {
			    setWidths();
			    repositionStickyHead();
			    repositionStickyCol();
			}))
			.scroll($.throttle(250, repositionStickyHead));
        }
    });
});