(function () {
    //filter cho combobox là dropdownlist
    function onItemFiltering(s, e) {
        if (!e.isFit)
            e.isFit = toLatin(e.item.text).indexOf(toLatin(e.filter)) > -1;
    }
    function onCustomHighlighting(s, e) {
        e.highlighting = e.filter;
    }
    function toLatin(text) {
        text = text.toLowerCase();
        text = text.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
        text = text.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
        text = text.replace(/ì|í|ị|ỉ|ĩ/g, "i");
        text = text.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
        text = text.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
        text = text.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
        text = text.replace(/đ/g, "d");
        return text;
    }
    //khi click vào textbox
    function onTextBoxInit(s, e) {
        var input = s.inputElement;
        setTimeout(function () {
            input.setSelectionRange(input.value.length, input.value.length);
        }, 1)
    }

    window.onItemFiltering = onItemFiltering;
    window.onCustomHighlighting = onCustomHighlighting;
    window.onTextBoxInit = onTextBoxInit;
})();

$(function () {
    "use strict";
    $(window).resize(function () {
        var modal = $('body').find('.modal[role="dialog"].show');

        if (modal.length == 0)
            return;

        var id = modal[0].id;

        resizeGridView(id);
    });

    $('.modal[role="dialog"]').on("shown.bs.modal", function (e) {

        var id = e.currentTarget.id;

        resizeGridView(id);
    });

    function resizeGridView(id) {
        var mbodys = $('#' + id).find('.modal-body .col-md-12 form');
        var m10bodys = $('#' + id).find('.modal-body .col-md-10 form');
        var m2bodys = $('#' + id).find('.modal-body .col-md-2 form');

        if (m10bodys.length > 0) {
            for (var i = 0; i < m10bodys.length; i++) {
                mbodys.push(m10bodys[i]);
            }
        }

        if (m2bodys.length > 0) {
            for (var i = 0; i < m2bodys.length; i++) {
                mbodys.push(m2bodys[i]);
            }
        }

        for (var m = 0; m < mbodys.length; m++) {

            var gViews = $(mbodys[m]).find('.grid-view');
            for (var i = 0; i < gViews.length; i++) {
                var gView = gViews[i].id;

                var width = mbodys[m].offsetWidth;
                var hsdcWidth = $('#' + gView + ' .dxgvHSDC')[0].style.paddingRight.replace("px", "");
                hsdcWidth = hsdcWidth == "" || hsdcWidth == undefined ? 0 : parseInt(hsdcWidth);

                $('#' + gView + ' .dxgvCSD').css("width", width);
                $('#' + gView + ' .dxgvHSDC div').css("width", width - hsdcWidth);
                $('#' + gView).css("width", width);
                $('#' + gView + ' .dxgvFCSD').css("width", width);
            }
        }
    }

    $('.modal[role="dialog"] .grid-view .dxgvCSD').scroll(function (e) {
        var parentElement = e.currentTarget.parentElement;
        if (e.currentTarget.scrollTop == 0) {
            $(parentElement).find('div.dxgvHSDC div').scrollLeft(e.currentTarget.scrollLeft);
        }
    });

    $('.modal[role="dialog"] .grid-view .dxgvFCSD').scroll(function (e) {
        var parentElement = e.currentTarget.parentElement;
        if (e.currentTarget.scrollTop == 0) {
            $(parentElement).find('div.dxgvHSDC div').scrollLeft(e.currentTarget.scrollLeft);
        }
    });
});