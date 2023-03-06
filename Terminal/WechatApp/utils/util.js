// 这个方法s是新建项目的时候自带的，暂时就放在这里
const formatTime = date => {
    const year = date.getFullYear()
    const month = date.getMonth() + 1
    const day = date.getDate()
    const hour = date.getHours()
    const minute = date.getMinutes()
    const second = date.getSeconds()

    return `${[year, month, day].map(formatNumber).join('-')} ${[hour, minute, second].map(formatNumber).join(':')}`
}

const formatNumber = n => {
    n = n.toString()
    return n[1] ? n : `0${n}`
}

// 按id合并列表

const distinctConcat = (a, b) => {
    let arr = a.concat(b)
    let result = []
    let obj = {}

    for (let i of arr) {
        if (!obj[i.id]) {
            result.push(i)
            obj[i.id] = 1
        }
    }

    return result
}

module.exports = {
    formatTime,
    distinctConcat
}