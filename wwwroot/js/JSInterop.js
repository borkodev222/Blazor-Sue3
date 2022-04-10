function downloadCSV(fileName, fileContent) {
    var link = document.createElement('a');
    link.download = fileName;
    link.href = "data:text/plain;charset=utf-8," + encodeURIComponent(fileContent)
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
function downloadBinary(fileName, fileContent) {
    var link = document.createElement("a");
    var blob = new Blob([fileContent], { type: "text/plain" });
    var url = window.URL.createObjectURL(blob);
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
function printResults() {
    var printContents = document.getElementById('test-results').innerHTML;
    var popupWin = window.open('', '_blank', 'width=800,height=10000,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no,top=50');
    popupWin.window.focus();
    popupWin.document.open();
    var html = '<!DOCTYPE html><html><head><title>Sue Test Results</title>'
        + '<style type="text/css">.table-result th {text-align: center;}.table-result .decision-col {width: 50%;}.table-result td {text-align: center;}</style>';

    var links = document.getElementsByTagName("link");
    for (var x = 0; x < links.length; x++) {
        if (links.item(x).rel.indexOf('stylesheet') != -1) {
            html += '<link rel="stylesheet" href="' + links.item(x).href + '" />';
        }
    }

    html = html + '</head><body onload="window.print(); window.close();"><div>'
        + printContents + '</div></html>';

    popupWin.document.write(html);
    popupWin.document.close();
}
function printModelInfo() {
    var printContents = document.getElementById('modelInfo').innerHTML;
    var popupWin = window.open('', '_blank', 'width=800,height=10000,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no,top=50');
    popupWin.window.focus();
    popupWin.document.open();
    var html = '<!DOCTYPE html><html><head><title>Model Info</title>'
        + '<style type="text/css">.table-result th {text-align: center;}.table-result .decision-col {width: 50%;}.table-result td {text-align: center;}</style>';

    var links = document.getElementsByTagName("link");
    for (var x = 0; x < links.length; x++) {
        if (links.item(x).rel.indexOf('stylesheet') != -1) {
            html += '<link rel="stylesheet" href="' + links.item(x).href + '" />';
        }
    }

    html = html + '</head><body onload="window.print(); window.close();"><div>'
        + printContents + '</div></html>';

    popupWin.document.write(html);
    popupWin.document.close();
}
function rememberUser(userId,password) {
    localStorage['sue-loginid'] = userId;
    localStorage['sue-logintoken'] = password;
}
function recallLoginId() {
    return localStorage.getItem('sue-loginid');
}
function recallLoginToken() {
    return localStorage.getItem('sue-logintoken');
}
window.clipboardCopy = {
    copyText: function (text) {
        navigator.clipboard.writeText(text).then(function () {
            alert("Copied to clipboard!");
        })
            .catch(function (error) {
                alert(error);
            });
    }
};