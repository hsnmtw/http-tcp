var html = {}
var http = {}
http.post = function(url,data,then){
    fetch(url,{method:'post',body:JSON.stringify(data)}).then(x=>x.json()).then(then).catch(then)
}
http.get = function(url,data,then){
    const urlArg = pair => {
        const[key,val]=pair
        if(Array.isArray(val) && val.length>1) 
            return val.map(v => [key,v]).map(urlArg).join('&')
        return `${key}=${encodeURIComponent(val)}` 
    }
    const args = (url.indexOf('?')>-1?'&':'?')+Object.keys(data).map(x=>[x,data[x]]).map(urlArg).join('&')
    fetch(url+args).then(x=>x.text()).then(then).catch(then)
}

html.load = (target,url) => {
    http.get(url,{},x=>{ html.set(target,x) })
}

html.view = (url) => {
    html.load('view',url)
}

html.each = function(selector,then){
    return Array.from(document.querySelectorAll(selector)).map(then)
}
html.set = function(selector,content){
    html.each(selector,x=>{
        x.innerHTML=content
        x.querySelectorAll('script').forEach(s => {
            const script = document.createElement('script')
            if(s.src) script.src = s.src
            if(s.defer) script.defer = s.defer
            if(s.type) script.type = s.type
            script.innerText = s.textContent
            document.body.appendChild(script)
        })
    })
}
html.data = function(selector) {
    const listSelector = 'input[name][type="checkbox"]:not([value="true"]):checked'
    const booleanSelector = 'input[name][type="checkbox"][value="true"]'
    const valueSelectors = [
        'input[name][type="text"]',
        'input[name][type="date"]',
        'input[name][type="time"]',
        'input[name][type="range"]',
        'input[name][type="hidden"]',
        'input[name][type="password"]',
        'input[name][type="radio"]:checked',
        'input[name]:not([type])',
        'textarea[name]',
        'select[name]',
    ].join(',')
    const result = {}
    const isArray = x => (x && typeof x === "object" && Array.isArray(x))
    const addToArray = (arr,v) => {
        if(!isArray(arr)) arr = []
        arr.push(v)
    }
    document.querySelectorAll(selector).forEach(f=>{
        f.querySelectorAll(valueSelectors).forEach(x=>result[x.name]=x.value)
        f.querySelectorAll(booleanSelector).forEach(x=>result[x.name]=c.checked)
        f.querySelectorAll(listSelector).forEach(x=>addToArray(result[x.name],x.value))
    })
    return result
}