module.exports = async function (context, req) {
    context.log('JavaScript HTTP trigger function processed a request.');

    if (req.Method == HttpMethod.Post) {
        input = {
            "userId": req.body.userId,
            "payload": req.body
        }
        context.bindings.updateSubmission = JSON.stringify(input)
    } else {
        userId = req.query.userId
        submissionId = req.query.submissionId
        if(submissionId == null) {
            document = context.bindings.allHistory 
        }
        else { 
            document = context.bindings.historyBySubmissionId
        }
        context.res = {
            // status: 200, /* Defaults to 200 */
            body: document
        }
    }
    
    
}