$.msg = {
    error: function (text) {
        $.messager.model = {
            ok: {text: '<i class="ace-icon fa fa-check"></i>确认', classed: 'btn-success btn-sm'},
        };
        $.messager.alert("错误", '<div class="huaduConfirm"><i class="fa fa-times-circle fa-5x red"></i></br></br>' + text + '</div>');
    },
    //waring: function (text) {
    //    $.messager.alert("警告", '<div class="huaduConfirm"><i class="fa fa-exclamation-triangle fa-5x orange2"></i></br></br>' + text + '</div>');
    //},
    alert: function (text) {
        $.messager.model = {
            ok: {text: '<i class="ace-icon fa fa-check"></i>确认', classed: 'btn-success btn-sm'},
        };
        $.messager.alert("提示", '<div class="huaduConfirm"><i class="fa fa-exclamation-triangle fa-5x orange2"></i></br></br>' + text + '</div>');
    },
    confirm: function (text, fn) {
        $.messager.model = {
            ok: {text: '<i class="ace-icon fa fa-check"></i>确认', classed: 'btn-success btn-sm'},
            cancel: {text: '<i class="ace-icon fa fa-times"></i>取消', classed: 'btn-default btn-sm'}
        };
        $.messager.confirm("确认", '<div class="huaduConfirm"><i class="fa fa-question-circle fa-5x orange2"></i></br></br> ' + text + '</div>', fn);
    },
    tip: function (text) {
        $.messager.model = {
            ok: {text: '<i class="ace-icon fa fa-check"></i>确认', classed: 'btn-success btn-sm'},
        };
        $.messager.alert("确认", '<div class="huaduConfirm"><i class="fa fa-exclamation-circle fa-5x blue"></i></br></br>' + text + '</div>');
    },
    show: function (text) {
        $.messager.popup("<span style='color:green;font-weight:bold;'>" + text + "</span>");
    }
};

window.onresize = function () {
    $("#huaduCover").height($(window).height()).css("line-height", $(window).height() + "px").width($(window).width());
};

/*
 $(document).ajaxStart(function () {
 showMask("数据加载中，请稍候");
 }).ajaxStop(function () {
 hideMask();
 });
 */

//显示遮照层
function showMask(text) {
    //$('body').css("overflow", "hidden")
    $("#huaduCover").show();
    $("#huaduProgressBar").html(text);
}

//隐藏遮照层
function hideMask() {
    //$('body').css("overflow", "")
    $("#huaduCover").hide();
}

function handleJQueryAjaxError(xhr, textStatus, error) {
    hideMask();
    if (textStatus === 'timeout') {
        $.msg.error('服务器超时！');
    } else {
        $.msg.error('访问服务器出错，请重试！' + error);
    }
}

$.ajaxSetup({
    "type": "POST",
    "timeout": 0,
    "cache": false,
    "error": handleJQueryAjaxError
});

function initDataTable(table, options) {

    var htmlTable = $(table);
    var dataTable = htmlTable.dataTable(options);

    function checkAll() {
        var thCheckBox = this;
        $(table).find('tbody tr>td:first-child input:checkbox').each(function () {
            var tdCheckBox = this;
            tdCheckBox.checked = thCheckBox.checked;
            var tableRow = $(this).closest('tr');
            if (tdCheckBox.checked) {
                tableRow.addClass('selected');
            } else {
                tableRow.removeClass('selected');
            }
        });
    }

    htmlTable.on('click', 'thead th input.checkall', checkAll);

    $(table + '_wrapper').on('click', 'div.dataTables_scroll div.dataTables_scrollHead thead th input.checkall', checkAll);

    /*
     htmlTable.on('click', 'tbody tr', function () {
     var tableRow = $(this);
     var checkbox = tableRow.find('td:first-child input:checkbox')[0];
     if (checkbox != undefined) {
     checkbox.checked = !checkbox.checked;
     if (checkbox.checked) {
     tableRow.addClass('selected');
     } else {
     tableRow.removeClass('selected');
     }
     }
     });
     */

    htmlTable.on('click', 'tbody td:first-child input:checkbox', function (event) {
        event.stopPropagation();
        var checkbox = $(this)[0];
        var tableRow = $(this).closest('tr');
        if (checkbox.checked) {
            tableRow.addClass('selected');
        } else {
            tableRow.removeClass('selected');
        }

        tableRow.removeClass('hilight');
    });

    return dataTable;
}

function initSingleSelectDataTable(table, options) {
    var htmlTable = $(table);
    htmlTable.dataTable(options);

    htmlTable.on('click', 'tbody tr', function () {
        var tableRow = $(this);
        var checkbox = tableRow.find('td:first-child input:checkbox')[0];
        if (checkbox !== undefined) {
            clearSelected(htmlTable);
            checkbox.checked = true;
            if (checkbox.checked) {
                tableRow.addClass('selected');
            }
        }
    });

    htmlTable.on('click', 'tbody td:first-child input:checkbox', function (event) {
        event.stopPropagation();
        var checkbox = $(this)[0];
        clearSelected(htmlTable, checkbox);
        var tableRow = $(this).closest('tr');
        if (checkbox.checked) {
            tableRow.addClass('selected');
        }
    });

    function clearSelected(htmlTable, exceptedCheckbox) {
        htmlTable.find('tbody tr').removeClass('selected');
        htmlTable.find('tbody td:first-child input:checkbox').each(function () {
            var checkbox = $(this)[0];
            if (exceptedCheckbox !== checkbox) {
                checkbox.checked = false;
            }
        });
    }
}

function initListTable(table) { // table name start with '#'

    var htmlTable = $(table);

    function checkTableAllRows() {
        var thCheckBox = this;
        $(table).find('tbody tr>td:first-child input:checkbox').each(function () {
            var tdCheckBox = this;
            tdCheckBox.checked = thCheckBox.checked;
            var tableRow = $(this).closest('tr');
            if (tdCheckBox.checked) {
                tableRow.addClass('selected');
            } else {
                tableRow.removeClass('selected');
            }
        });
    }

    htmlTable.on('click', 'thead th input.checkall', checkTableAllRows);

    htmlTable.on('click', 'tbody td:first-child input:checkbox', function (event) {
        event.stopPropagation();
        var checkbox = $(this)[0];
        var tableRow = $(this).closest('tr');
        if (checkbox.checked) {
            tableRow.addClass('selected');
        } else {
            tableRow.removeClass('selected');
        }

        tableRow.removeClass('hilight');
    });
}

function initSingleSelectListTable(table) {

    var htmlTable = $(table);

    htmlTable.on('click', 'tbody td:first-child input:checkbox', function (event) {
        event.stopPropagation();
        var checkbox = $(this)[0];
        clearSelected(htmlTable, checkbox);
        var tableRow = $(this).closest('tr');
        if (checkbox.checked) {
            tableRow.addClass('selected');
        }
    });

    function clearSelected(htmlTable, exceptedCheckbox) {
        htmlTable.find('tbody tr').removeClass('selected');
        htmlTable.find('tbody td:first-child input:checkbox').each(function () {
            var checkbox = $(this)[0];
            if (exceptedCheckbox !== checkbox) {
                checkbox.checked = false;
            }
        });
    }
}

function getSelectIdList(table) { // table name start with '#'
    var id_list = [];
    $(table + ' tbody tr>td:first-child input:checkbox').each(function () {
        if (this.checked) {
            id_list.push($(this).val());
        }
    });

    return id_list;
}

function trim(str) { //删除左右两端的空格
    return str.replace(/(^\s*)|(\s*$)/g, "");
}

//bootstrap modal options
var modalOptions = {backdrop: 'static', keyboard: false};

$.extend($.validator.messages, {
    required: "必填",
    remote: "请修正该字段",
    email: "请输入正确格式的电子邮件",
    url: "请输入合法的网址",
    date: "请输入合法的日期",
    dateISO: "请输入合法的日期(ISO).",
    number: "请输入合法的数字",
    digits: "只能输入整数",
    creditcard: "请输入合法的信用卡号",
    equalTo: "请再次输入相同的值",
    accept: "请输入拥有合法后缀名的字符串",
    maxlength: jQuery.validator.format("请输入一个长度最多是 {0} 的字符串"),
    minlength: jQuery.validator.format("请输入一个长度最少是 {0} 的字符串"),
    rangelength: jQuery.validator.format("请输入一个长度介于 {0} 和 {1} 之间的字符串"),
    range: jQuery.validator.format("请输入一个介于 {0} 和 {1} 之间的值"),
    max: jQuery.validator.format("请输入一个最大为 {0} 的值"),
    min: jQuery.validator.format("请输入一个最小为 {0} 的值")
});

function validate(form, rules, errorPlacement) {
    if (errorPlacement === undefined) {
        form.validate({
            submitHandler: function () {
            },
            rules: rules,
            highlight: function (e) {
                $(e).closest('.form-group').addClass('has-error');
            },
            success: function (e) {
                $(e).closest('.form-group').removeClass('has-error');
                $(e).remove();
            },
            errorElement: 'div',
            errorClass: 'help-block inline err-div',
            focusInvalid: false,
            ignore: "",
            errorPlacement: function (error, element) {
                var parent = element.parent("div");
                if (parent.hasClass('input-group')) {
                    parent = parent.parent();
                }
                error.appendTo(parent);
            }
        });
    } else {
        form.validate({
            submitHandler: function () {
            },
            rules: rules,
            highlight: function (e) {
                $(e).closest('.form-group').addClass('has-error');
            },
            success: function (e) {
                $(e).closest('.form-group').removeClass('has-error');
                $(e).remove();
            },
            errorElement: 'div',
            errorClass: 'help-block inline err-div',
            focusInvalid: false,
            ignore: "",
            errorPlacement: errorPlacement
        });
    }

    return form.valid();
}

$.fn.clearValidation = function () {
    $(this).find("div.has-error").removeClass('has-error');
    $(this).find("div.err-div").remove();
};

function isEmptyStr(str) {
    if (!str || str.length === 0 || str === "" || typeof str === 'undefined' || !/[^\s]/.test(str) || /^\s*$/.test(str) || str.replace(/\s/g, "") === "") {
        return true;
    } else {
        return false;
    }
}

function downLoadFile(options) {
    var config = $.extend(true, {method: 'post'}, options);
    var $form = $('<form method="' + config.method + '" />');
    $form.attr('action', config.url);
    for (var key in config.data) {
        $form.append('<input type="hidden" name="' + key + '" value="' + config.data[key] + '" />');
    }
    $(document.body).append($form);
    $form[0].submit();
    $form.remove();
}