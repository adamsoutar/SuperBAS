#[derive(Default)]
pub struct SuperBASProgram {
    /*DECLARATIONS*/
}

impl SuperBASProgram {
    fn go_sub(&mut self, raw_line: f64) {
        let mut line = raw_line;
        loop {
            if line == -1.0 {
                return
            } /*BODY*/
            else {
                panic!("Can't GOTO {} - invalid line number.", line)
            }
        }
    }

    fn execute (&mut self) {
        self.go_sub(/*LOWESTLINE*/f64)
    }
}

fn main() {
    let mut program = SuperBASProgram::default();
    program.execute();
}
