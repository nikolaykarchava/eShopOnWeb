module.exports = async function (context, req) {
    try{
        context.log('JavaScript HTTP trigger function processed a request.');

        context.bindings.deliveryOrder = JSON.stringify(req.body)

        context.res = {};
    } catch(e) {
        context.log(JSON.stringify(e))
    }
    
}