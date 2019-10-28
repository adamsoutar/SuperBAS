using System;

namespace SuperBAS.Transpiler.Javascript
{
    public static class Skeleton
    {
        // As someone who writes JS, this pains me.
        public static string Code = @"
/*DEFINITIONS*/
let stop = false
const sleep = time => new Promise(resolve => setTimeout(resolve, time))

async function goSub (ln) {
  while (!stop) {
      switch (ln) {
        case -1:
            return;
/*CASES*/
        default:
            throw new Error(`Invalid GOTO ${ln} - Not a line`)
            return
      }
  }
}

goSub(/*LOWESTLINE*/)
        ";
    }
}
