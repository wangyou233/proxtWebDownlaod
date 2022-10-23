
class Hub {

    constructor() {
        this.connection = null;
        this.startedPromise = null;
        this.manuallyClosed = false;
    }

    startSignalR(jwtToken) {

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(
                `/download/hub`,
                jwtToken ? {accessTokenFactory: () => jwtToken} : null
            )
            .build()
        this.startedPromise = this.connection.start()
            .catch(err => {
                console.error('Failed to connect with hub', err)
                return new Promise((resolve, reject) => setTimeout(() => this.start().then(resolve).catch(reject), 5000))
            })
    }

    subscribe(sessionId) {
        if (this.manuallyClosed === true) return true;
        let that = this;
        setTimeout(function () {
            that.connection.invoke("Subscribe", sessionId)
        }, 2000)

    }

    downloadFileStatus(fn) {
        if (this.manuallyClosed === true) return true;
        this.connection.on('DownloadFileStatus', (reason) => {
            fn(reason)
        })
    }

    /**
     * 接受信息
     * @param fn
     */
    downloadFileCompletion(fn) {
        if (this.manuallyClosed === true) return true;
        this.connection.on('DownloadFileCompletion', (message) => {
            fn(message)
        })
    }

    stopSignalR() {
        if (!this.startedPromise) return
        this.manuallyClosed = true
        return this.startedPromise
            .then(() => this.connection.stop())
            .then(() => {
                this.startedPromise = null
            })
    }

    start() {
        this.startedPromise = this.connection.start()
            .catch(err => {
                console.error('Failed to connect with hub', err)
                return new Promise((resolve, reject) => setTimeout(() => start().then(resolve).catch(reject), 5000))
            })
        return this.startedPromise
    }

}

