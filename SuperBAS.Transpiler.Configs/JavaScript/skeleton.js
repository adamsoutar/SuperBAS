/*DECLARATIONS*/
let stop = false
const sleep = (ms) => new Promise(resolve => setTimeout(resolve, ms))

async function goSub (line) {
  switch (line) {
    case -1:
      return
    /*BODY*/
    break
    default:
      throw new Error(`Can't GOTO ${line} - invalid line number.`)
  }
}

function readAllFile () {
  console.log('READALLFILE is not yet supported in JS.')
}

function writeAllFile (flName, contents, append) {
  console.log('WRITEALLFILE is not yet supported in JS.')
}

function newDim (...dimensions) {

}
function getArrayValue (array, ...dimensions) {

}

goSub(/*LOWESTLINE*/)
