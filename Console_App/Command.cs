using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_App {
    public class Command {
        // private
        string _name;
        string _description;
        string[] _args;
        Action<string[]> _exec;

        // public
        public string Name { get { return _name; } }
        public string Description { get { return _description; } }
        public string[] Args { get { return _args; } }
        public Command(string name, string description, string[] args, Action<string[]> exec) {
            // args here, is just a description of what are expected arguments
            _name = name;
            _description = description;
            _args = args;
            _exec = exec;
        }

        public void Execute(string[] args) {
            // args here are 100% natural arguments
            _exec(args);
        }
    }
}
