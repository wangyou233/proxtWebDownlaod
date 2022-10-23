$(document).ready(function () {
    $("#submitDownload").click(function () {
        $(".downloadUrlBox").hide()

        let downloadUrl = $("#downloadUrl").val();
        let isProxy = $("#flexSwitchCheckChecked").val();
        if (downloadUrl === undefined) {
            alert("The download address cannot be empty");
        }
        let data = {
            isProxy: isProxy === "on",
            // isProxy: false,
            downloadFileUrl: downloadUrl
        }
        $.ajax({
            type: "POST",
            url: "/api/download",
            data: data,

            success: function (data) {
                if (data !== null) {
                    $("#submitDownload").attr("disabled", true)
                    let hub = new Hub()
                    hub.startSignalR()
                    hub.subscribe(data)
                    hub.downloadFileCompletion(function (res) {
                        console.log(res)
                        $("#submitDownload").attr("disabled", false)
                        if (res.isSuccess) {
                            let dom = $("#progressbar");
                            $(".downloadUrlBox").show()
                            let downloadDom = $("#download");
                            downloadDom.attr("href", res.downloadPath)
                            downloadDom.text(res.downloadPath)
                            $("#dateTime").text(res.dateTime)
                            dom.text(100 + "%")
                            dom.css("width", 100 + "%")
                        } else {
                            alert(res.msg)

                        }

                        hub.stopSignalR()

                    })
                    hub.downloadFileStatus(function (res) {
                        let dom = $("#progressbar");
                        $("#progressbarBox").show();
                        dom.show()
                        dom.text(res.progress + "%")
                        dom.css("width", res.progress + "%")
                        $("#msBox").show()
                        $("#ms").text(`Size：${(res.totalDownloadSize / 1024 / 1024)} MB/${res.totalSize / 1024 / 1024}MB`)

                    })
                }

            },
            failure: function (errMsg) {
                alert(errMsg);
            }
        });

    });
});