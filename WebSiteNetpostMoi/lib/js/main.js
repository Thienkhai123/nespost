$(document).ready(function () {
    local = location.href;
    var lstMenu = $('#navbarTogglerDemo03 .navbar-nav').find('.nav-item');
    $('#navbarTogglerDemo03 .navbar-nav').find('.nav-item').removeClass("active");

    if (screen.width <= 900 && location.pathname.indexOf("Tin_Tuc") == -1 && location.pathname.indexOf("Tuyen_Dung") == -1) {
        $('#banner').show();
    }

    for (var i = 0; i < lstMenu.length; i++) {
        var navlink = lstMenu[i].children;
        var path = navlink[0].pathname;
        if (local.indexOf("Bang_Gia") != -1) {
            $(lstMenu[2]).addClass("active");
        }
        else if (location.pathname == "/" || location.pathname.indexOf("Home") != -1) {
            $(lstMenu[0]).addClass("active");
        }
        else if (location.pathname.indexOf("Dich_Vu") != -1) {
            $(lstMenu[1]).addClass("active");
        }
        else if (local.indexOf("Tin_Tuc") != -1) {
            $(lstMenu[3]).addClass("active"); 
        }
        else if (local.indexOf("Lien_He") != -1) {
            $(lstMenu[6]).addClass("active");
        }
        else if (location.pathname == path) {
            $(lstMenu[i]).addClass("active");
        }
    }

    var header = document.getElementById("access");
    var sticky = header.offsetTop;

    $(window).scroll(function () {
        if ($(this).scrollTop() != 0) {
            $('#back-to-top').fadeIn(400, 'swing');
        } else {
            $('#back-to-top').fadeOut(400, 'swing');
        }

        if (window.pageYOffset > sticky) {
            header.classList.add("sticky");
        } else {
            header.classList.remove("sticky");
        }
    });

    $('img').bind('cut copy paste contextmenu', function (e) {
        if (e.currentTarget.parentNode.className != "viewer iviewer_cursor") {
            e.preventDefault();
        }
    });

    $(".dmenu").click(
        function () {
            if ($('.lstdv')[0].style.display == "block") {
                $('#sortdown').removeClass('fas fa-sort-up').addClass('fas fa-sort-down');
                $('.dmenu .nav-link').removeClass("hover");
                $('.lstdv').hide();
            }
            else {
                $('#sortdown').removeClass('fas fa-sort-down').addClass('fas fa-sort-up');
                $('.dmenu .nav-link').addClass("hover");
                $('.lstdv').show();
            }
        }
    );

    if (screen.width > 600) {
        $(".dmenu").hover(
            function () {
                $('#sortdown').removeClass('fas fa-sort-down').addClass('fas fa-sort-up');
                $('.dmenu .nav-link').addClass("hover");
                $('.lstdv').fadeIn();
            }, function () {
                $('#sortdown').removeClass('fas fa-sort-up').addClass('fas fa-sort-down');
                $('.dmenu .nav-link').removeClass("hover");
                $('.lstdv').fadeOut();
            }
        );
    }

    $(".dmenu_bang_gia").click(
        function () {
            if ($('.lst_bang_gia')[0].style.display == "block") {
                $('#icon_sort_bang_gia').removeClass('fas fa-sort-up').addClass('fas fa-sort-down');
                $('.dmenu_bang_gia .nav-link').removeClass("hover");
                $('.lst_bang_gia').hide();
            }
            else {
                $('#icon_sort_bang_gia').removeClass('fas fa-sort-down').addClass('fas fa-sort-up');
                $('.dmenu_bang_gia .nav-link').addClass("hover");
                $('.lst_bang_gia').show();
            }
        }
    );

    if (screen.width > 600) {
        $(".dmenu_bang_gia").hover(
            function () {
                $('#icon_sort_bang_gia').removeClass('fas fa-sort-down').addClass('fas fa-sort-up');
                $('.dmenu_bang_gia .nav-link').addClass("hover");
                $('.lst_bang_gia').fadeIn();
            }, function () {
                $('#icon_sort_bang_gia').removeClass('fas fa-sort-up').addClass('fas fa-sort-down');
                $('.dmenu_bang_gia .nav-link').removeClass("hover");
                $('.lst_bang_gia').fadeOut();
            }
        );
    }

    $('#tracuuvandon').click(function () {
        $(".dr-tracuu").fadeIn();
    });
});

function scrollTop() {
    if (screen.width < 640) {
        $('html,body').animate({ scrollTop: (document.body.scrollHeight - 800) }, 600);
    }
    else if (screen.width > 640 && screen.width < 1280) {
        $('html,body').animate({ scrollTop: 1000 }, 600);
    }
    else {
        $('html, body').animate({
            scrollTop: 510
        }, 600);
    }
    $('#kamotsu1').focus();
}

//function currentSlide(n, id) {
//    showSlides(n, slideIndex = id);
//    $('#myModal').modal("show");
//}

//function plusSlides(n, id) {
//    showSlides(n, slideIndex += id);
//}

//var slideIndex = 1;
//function showSlides(n, id) {
//    var i;
//    var div;
//    var imglist = document.getElementsByClassName("imglist");
//    for (i = 0; i < imglist.length; i++) {
//        imglist[i].style.display = "none";
//    }
//    if (n == "air-freight") {
//        div = document.getElementById("img-air-freight");
//    }
//    if (n == "ocean-export") {
//        div = document.getElementById("img-ocean-export");
//    }
//    if (n == "gia_tri_cao") {
//        div = document.getElementById("img-gia_tri_cao");
//    }
//    if (n == "trucking") {
//        div = document.getElementById("img-trucking");
//    }
//    if (n == "tiet_kiem") {
//        div = document.getElementById("img-tiet_kiem");
//    }
//    if (n == "inspection-metering-services") {
//        div = document.getElementById("img-inspection-metering-services");
//    }
//    if (n == "removal") {
//        div = document.getElementById("img-removal");
//    }
//    if (n == "project-cargo") {
//        div = document.getElementById("img-project-cargo");
//    }
//    if (n == "thongtin") {
//        div = document.getElementById("img-thongtin");
//    }
//    if (n == "SGX") {
//        div = document.getElementById("img-SGX");
//    }
//    div.style.display = "block";
//    var slides = div.getElementsByClassName("mySlides");
//    if (id > slides.length) { slideIndex = 1 }
//    if (id < 1) { slideIndex = slides.length }
//    for (i = 0; i < slides.length; i++) {
//        slides[i].style.display = "none";
//    }
//    slides[slideIndex - 1].style.display = "block";
//}

//$("#myModal").on('hide.bs.modal', function () {
//    slideIndex = 1;
//});

