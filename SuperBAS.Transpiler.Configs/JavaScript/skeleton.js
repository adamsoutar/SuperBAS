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

function readAllFile (flName) {
  return localStorage.getItem(`basfile_${flName}`)
}

function writeAllFile (flName, contents, append) {
  const path = `basfile_${flName}`
  if (append) {
    contents += localStorage.getItem(path) || ''
  }
  localStorage.setItem(path, contents)
}

function mappable(n) {
  let out = []
  for (let i = 0; i < n; i++) out.push(0)
  return out
}

function newDim (isString, ...dims) {
  return dimComp(isString, dims, 0)
}
// Recursive method for producing multi-dimensional arrays
function dimComp (isString, dims, i) {
  if (i != dims.length - 1) {
    return mappable(dims[i]).map(x => dimComp(isString, dims, i + 1))
  } else {
    return mappable(dims[i]).map(x => isString ? '' : 0)
  }
}

function getArrayValue (array, ...dims) {
  let val = array
  for (let i = 0; i < dims.length; i++) {
    if (dims[i] > val.length) {
      // We *cannot* leak undefined/null into the SuperBAS type system
      throw new Error(`[SuperBAS] - Index ${dims[i]} in variable(${dims.join(', ')}) was outside the bounds of the array/list.`)
    }
    val = val[dims[i]]
  }
  return val
}

goSub(/*LOWESTLINE*/)
