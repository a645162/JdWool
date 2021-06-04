
var Id;
var Begin;

$(function () {
    JdLoginQrCode("qrcode");
});

function JdLoginQrCode(id) {
    $.get("/JdLogin/GetTmauth", function (res) {
        RefreshQrCode(id, res.tmauth);
        Id = res.id

        Begin = setInterval(function () {
            GetJdCookie();
        }, 2000);
    })
}

function RefreshQrCode(id, text) {
    document.getElementById(id).innerText = "";
    new QRCode(document.getElementById(id), {
        text: text,
        width: 200,
        height: 200,
        colorDark: "#111111",
        colorLight: "#ffffff",
        correctLevel: QRCode.CorrectLevel.H
    });
}


function GetJdCookie() {
    $.get("/JdLogin/GetCookie?id=" + Id, function (res) {
        if (res.code == "0") {
            var CookieValue = res.cookie.match(/pt_pin=.+?;/) + res.cookie.match(/pt_key=.+?;/);
            $("#cookie").val(CookieValue);

            clearInterval(Begin);
        }
    })
}

function CopyCookie() {
    var oInput = document.createElement('input');
    oInput.value = $("#cookie").val();
    document.body.appendChild(oInput);
    oInput.select();
    document.execCommand("Copy");
    oInput.className = 'oInput';
    oInput.style.display = 'none';

    ShowAlert("succes", "复制成功");
}


// 显示警告框
function ShowWarning(text) {
    ShowAlert("warning", text);
}

// 显示成功框
function ShowSuccess(text) {
    ShowAlert("succes", text);
}

// 根据自定义的提示
function ShowAlert(type, text) {
    let ac = document.getElementById("alert-container");

    if (!ac) {
        let elementDiv = document.createElement("div");
        elementDiv.id = "alert-container";
        elementDiv.style = "position:absolute;width:300px;right:30px;top:20px;"
        document.body.appendChild(elementDiv);
        ac = document.getElementById("alert-container");
    }
    let alert = document.createElement("div");

    switch (type) {
        case "succes":
            alert.innerHTML = "<strong>成功：</strong>" + text;
            alert.className = "alert alert-success alert-dismissible fade show";
            break;
        case "warning":
            alert.innerHTML = "<strong>警告：</strong>" + text;
            alert.className = "alert alert-warning alert-dismissible fade show";
            break;
        default:
            alert.innerHTML = "<strong>成功：</strong>" + text;
            alert.className = "alert alert-success alert-dismissible fade show";
            break;
    }
    ac.appendChild(alert);
    setTimeout(function () {
        ac.removeChild(alert);
    }, 2000);
}