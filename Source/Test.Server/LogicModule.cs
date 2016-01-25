using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Shared;

namespace Test.Server
{
    public class LogicModule
    {
        private List<LogicRule> Rules { get; } = new List<LogicRule>
        {
            new LogicRule() {Name = "Commutative",      Rule1 = "p ∧ q ⇐⇒ q ∧ p",                       Rule2 = "p ∨ q ⇐⇒ q ∨ p"},
            new LogicRule() {Name = "Associative",      Rule1 = "(p ∧ q) ∧ r ⇐⇒ p ∧ (q ∧ r)",          Rule2 = "(p ∨ q) ∨ r ⇐⇒ p ∨ (q ∨ r)"},
            new LogicRule() {Name = "Distributive",     Rule1 = "p ∧ (q ∨ r) ⇐⇒ (p ∧ q) ∨ (p ∧ r)",    Rule2 = "p ∨ (q ∧ r) ⇐⇒ (p ∨ q) ∧ (p ∨ r)"},
            new LogicRule() {Name = "Identity",         Rule1 = "p ∧ T ⇐⇒ p",                            Rule2 = "p ∨ F ⇐⇒ p"},
            new LogicRule() {Name = "Negation",         Rule1 = "p∨ ∼ p ⇐⇒ T",                          Rule2 = "p∧ ∼ p ⇐⇒ F"},
            new LogicRule() {Name = "Double Negative",  Rule1 = "∼ (∼ p) ⇐⇒ p"},
            new LogicRule() {Name = "Idempotent",       Rule1 = "p ∧ p ⇐⇒ p",                           Rule2 = "p ∨ p ⇐⇒ p"},
            new LogicRule() {Name = "Universal Bound",  Rule1 = "p ∨ T ⇐⇒ T",                           Rule2 = "p ∧ F ⇐⇒ F"},
            new LogicRule() {Name = "De Morgan’s",      Rule1 = "∼ (p ∧ q) ⇐⇒ (∼ p) ∨ (∼ q)",         Rule2 = "∼ (p ∨ q) ⇐⇒ (∼ p) ∧ (∼ q)"},
            new LogicRule() {Name = "Absorption",       Rule1 = "p ∨ (p ∧ q) ⇐⇒ p",                    Rule2 = "p ∧ (p ∨ q) ⇐⇒ p"},
            new LogicRule() {Name = "Conditional",      Rule1 = "(p =⇒ q) ⇐⇒ (∼ p ∨ q)",              Rule2 = "∼ (p =⇒ q) ⇐⇒ (p∧ ∼ q)}"},
        };

    }
}
