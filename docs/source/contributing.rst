**********************
Contributing to KORE
**********************

Welcome to the KORE (Kerbo Operations Runtime Engine) development community! This section is for contributors who want to help develop, improve, or extend the project.

.. note::
   If you're just looking to use KORE for fun projects or learning, check out the main documentation sections. This area focuses on project development and contribution guidelines.

Project Vision & Goals
======================

**First and foremost, KORE is a mod for Kerbal Space Program.**

This means in no uncertain terms that the primary goal of the project is to be a fun and engaging mod for Kerbal Space Program. Period.
Second is that as we are using real computing concepts, and architectures, we have an opportunity to teach people real computer science, and real software engineering for fun and profit in the real world.

What does this mean for us?

* We need to keep in mind that the main purpose of KORE is to be used as a mod for Kerbal Space Program.
* We are a mod for games, games are for fun, so we first and foremost need to make the gameplay as fun as possible.
* We are not just making a blackbox project that no one can see the code of, we are making a project that is open and transparent and that anyone can learn from and importantly may very well read through the code to learn more about computer science and software engineering.
  * This means that we need to write code that is clean, professional and easy to understand.
  * It also means that we need to write documentation that is clear, concise and easy to understand.
  * It also means that we need to write tests that are comprehensive and easy to understand. (Not just so we make good code, but also so that if someone wants to start contributing, they have a place to start and a way to understand the codebase and guiderails to follow.)
* We are not just talking about computer science, but also about software engineering, and that the codebase should be as clean and professional as possible.

**Educational Mission**

While we say that KORE is first and formost a mod for fun at the end of the day, KORE's primary goal is teaching computer science concepts through practical, hands-on implementation. We believe that learning is most effective when students can see real systems working, not just toy examples or simplified demonstrations.
While KORE is intended to be used as a platform for learning, it is also a real software product that can be used to create real software, but more importantly it is a mod for Kerbal Space Program and maybe other games in the future.

No one likes educational software that forgets to first be a fun to use, and no one likes games that are not fun to play.
Too many educational software projects forget that they need to be fun to use, and too many game developers forget that they need to be educational.
KORE is designed to be both fun to use and educational, and to make the learning experience as smooth and enjoyable as possible.

Far too many educational games feel like when HR comes into the meeting and informs you that they are going to be doing a "fun" activity.
Does anyone really like those manditory corporate fun sessions? I'm sure there are some, but I'm sure they are not the majority. But everyone fixes a smile and pretends to enjoy it. (No offense to HR, I'm sure they are nice people, but that's not the point.) The point is that it's not fun.

KORE is a Game mod that people can learn real skills from but is something that people choose to add to their game and play. 
No one is being forced to use it and so let's not make it feel like a chore.

**Gaming Integration**

The whole point is that we're making computer science concepts actually engaging by putting them in a game people already love. When you're programming a rocket's guidance system in KSP, you're not just learning about :term:`symbol` management - you're trying to get Jeb home safely! That's way more motivating than "Exercise 4.2: Write a function that demonstrates symbol visibility."

We get immediate visual feedback too. Your code either works (rocket goes to space) or it doesn't (rocket goes boom). There's something deeply satisfying about watching your assembly code actually control a spacecraft. But oddly enough, sometimes it's just as fun to watch the rocket go boom. 
This takes the edge off of failure and helps people learn from their mistakes without getting too stuck in the conceptual weeds.

**Real Architecture**

Here's the thing - we could have made up some simplified "educational" instruction set that's easier to implement. But then you'd learn our made-up system instead of something you can actually use in the real world.

We use actual RISC-V because when you finish learning with KORE, you'll know a real architecture that's being used in actual products. Your skills transfer directly to professional development and computer engineering. No "toy" languages here.

If you learn to program in KORE at the embedded level, you'll know a real architecture that's being used in actual products. For example, the very popular ESP32 microcontroller has a variant ESP32-C3 that uses the RISC-V architecture. 
(Ohh but I bet it's super expensive, right? Nope, it's dirt cheap and you can get it on a breadboard for less than $5! At the time of this writing on Amazon it's about $10 for a single chip/ $30 for a set of 10 boards (This is $3 per board, and you can't get a lot of other stuff for $3!(And there is currently a trade war on, and they are still do cheap)) /or even a set of 2 boards with little OLED screens on them for $16.)

**Progressive Learning Path**

KORE lets you start wherever you're comfortable and go as deep as you want:

1. **High-level programming** (planned KS++ language) - "I just want to make my rocket do cool things"
2. **Virtual machine concepts** - "How does this high-level stuff actually work?"
3. **Assembly programming** (RISC-V assembly with KUICK) - "I want to understand what the computer is really doing"
4. **Machine code** (binary instruction encoding) - "Show me the actual 1s and 0s"
5. **Hardware concepts** (CPU simulation) - "I want to build my own processor" (Sorta, but not really. You can learn about it though and that's what matters)

Nobody has to climb the whole ladder, but it's there if you want to.

Documentation Philosophy
========================

Look, documentation is usually boring. We're trying to make it not boring.

**Plain English Sections**

Every time we explain something complicated, we include a "Plain English" section. Think of it as the "explain like I'm five" version, but without being condescending.

These sections should:

- Use analogies that actually make sense (street addresses for :term:`symbol names`)
- Explain why you'd care about this problem in the first place
- Show real examples, not contrived textbook nonsense
- Connect to stuff you might actually want to do

**Technical Precision with Accessibility**

Here's our trick: we use analogies but we don't hide the real terms. Instead of just saying "think of it like a street address," we say "think of it like a street address (:term:`symbol names`)."

.. code-block:: rst

   Think of labels like street addresses (:term:`symbol names`)

This way you learn the concept AND the proper vocabulary. When you go to a job interview, you can talk about :term:`symbol tables` and :term:`linkage`, not just "those address thingies."

**Glossary Integration**

Every technical term gets defined in our :doc:`glossary` and linked with ``:term:`term_name``` syntax. This means:

- You can click on any term to see what it means
- All our definitions are consistent across the whole project
- New contributors can learn the vocabulary as they go
- It looks professional (which it is)

**Code Examples**

Every feature needs examples that show:

- Syntax definition with parameters table - "What the syntax looks like"
- Multiple usage examples - "How you'd actually use it"
- One step down explanations - "What happens behind the scenes"
- How it fits with everything else

Avoid "foo" and "bar" examples. Real code that does real things.

Code Quality Standards
======================

**Testing Everything**

It is important to test everything. Make sure we allways have tests for everything!
- Every parser feature needs corresponding tests
- Tests should cover both positive and negative cases
- Use descriptive test names that explain the scenario
- Follow the existing test file organization

We test everything. Not because we're obsessed with metrics, but because:

- Perdictable Expected and Acurate Behavior - "We don't want our users to think it's a bug in their code, only to go to the forums and find out we implimented something incorrectly."
- Tests catch bugs before users do
- Tests show new contributors how things are supposed to work
- Tests let us refactor fearlessly
- Good tests are documentation that never gets out of date

Every feature needs tests that cover the happy path and the ways things can go wrong.

**Test Driven Development**

We use test driven development to make sure we have tests for everything.
If we are fixing a bug, we write a test that shows the bug and then and only then fix the bug.

**Clear Code**

Write code like someone else will read it at 2 AM while trying to fix a critical bug. Because they will.

- Use names that explain what things do
- AST nodes have descriptive names (``DirectiveNode``, not ``Node1``)
- Comments explain *why*, not *what* "var i = 0; // Set i to 0" Is totally useless to everyone. How ever "var i = 0; // Reset the counter" is useful to new developers. But even better would be if you can make it so the code is helpful to new developers and experienced developers alike.
- If you need a comment to explain *what* the code does, the code probably needs to be clearer

**Documentation-First Development**

Before you write any code:

1. Add the task to KANBAN.md if it is not already there (so we know it exists, do this as soon as possible even when you can't work on it yet so we capture all planed work)
2. Move the task your working on to "In Progress" under your name (so we know who is working on it)
3. Write the documentation (forces you to think through the design)
4. Write tests based on the documentation, if they don't exist yet (defines what "done" looks like)
5. Write code to make the tests pass (Don't cheat though no hard coding allowed! or anything else. If your using AI to help you, make sure it is not cheating for you! AI LOVE TO JUST COMMENT OUT A TEST or sometimes they will be a little more creative and store an extra copy of the data and return it to pass the test.)
6. IF YOU ARE USING AI, You must always read and understand every line of code it writes on your behalf. You are responsible for the code it writes on your behalf. This means NO VIBE CODING!
5. Update any other documentation that changed

This feels backwards if you're used to "code first, document later" (which usually becomes "code first, document never"). But it results in better designs and fewer surprises.

**Cross-Platform Compatibility**

KORE works on both Linux and Windows. Keep it that way. Use the existing build scripts and test on both platforms when possible (Though losts of automated testing should help midigate this issue).

Contribution Guidelines
=======================

**KANBAN Workflow**

We track everything in KANBAN.md. It's not fancy, but it works, and more importantly it locks our project tracking to the codebase. Issues are for bugs reported by users, and we use KANBAN for all project management and pending work tasks.

Use this format:

.. code-block:: text

   * [COMPONENT][SUBCOMPONENT] Description of task

Examples:
- ``[KUICK][PARSER] Implement .section directive``
- ``[DOCS][ASSEMBLY] Add plain English section for labels``

**Documentation Standards**

Every contribution needs documentation. No exceptions. But don't panic - it doesn't have to be perfect, and we'll help you improve it.

What we need:
- RST format (it's like Markdown but more powerful)
- Glossary entries for new technical terms
- Plain English sections for complex concepts
- Code examples that show real usage
- Notes about how it integrates with existing features

**Testing Requirements**

- New features need new tests
- Don't break existing tests
- Test names should explain what they're testing
- Cover the edge cases where things might go wrong
- Follow the patterns in existing tests

**Code Review Focus**

When we review your code, we're looking at:

1. **Is this actually useful?** Does it solve a real problem?
2. **Can people learn from this?** Is it educational?
3. **Is it documented well?** Can someone else understand and use it?
4. **Is it tested?** How do we know it works? Or more importantly how do we know it still works correctly? We need to know if something else breaks it.
5. **Does it fit?** Does it match the existing architecture and style?

We're not trying to be mean. We want your contribution to succeed and help the project.

Architecture Decisions
======================

**Why RISC-V**

We picked RISC-V because it's:

- Actually used in real world products (not just academic)
- Open source (no licensing weirdness)
- Simple enough to understand
- Complex enough to be interesting
- Supported by real tools

**Multi-Layer Design**

The whole system is like an onion - you can peel back layers to see how things work:

.. code-block:: text

   High-Level Language (KS++) ← "I want to land on Mun"
   ↓
   Virtual Machine Code ← "Translate to intermediate form"
   ↓
   Assembly Language (KUICK) ← "RISC-V assembly instructions"
   ↓
   Machine Code ← "Actual binary the CPU executes"
   ↓
   Hardware Simulation ← "Simulated CPU running the code"

Each layer teaches different concepts and allows us to add diffrent gameplay challenges, and you can stop at whatever level interests you.

**Modular Structure**

We're splitting KUICK into its own library because:

- Other projects can use it
- It teaches software architecture concepts
- It demonstrates proper abstraction
- It makes the codebase easier to understand

Getting Started for Contributors
===============================

**Development Environment**

1. Follow the setup in the main README (it should work on your platform)
2. Make sure you can build and run tests
3. Poke around the codebase to get familiar with it
4. Read recent KANBAN entries to see what we're working on

**Understanding the Codebase**

Key places to look:
- ``src/KoreLibrary/`` - The core of the project, where the magic happens. This is where we actually implement the simulation of the RISC-V processor, busses, memory, and other components.
- ``src/Kore.AST/`` - How we represent programs internally
- ``src/Kore.Kuick.Tests/`` - Examples of how everything works
- ``docs/source/`` - Documentation structure and examples
- ``KANBAN.md`` - Project history and current tasks

**First Contribution Ideas**

Good ways to get started:

1. **Documentation improvements** - Add Plain English sections, fix typos, clarify confusing parts
2. **Test additions** - Improve coverage, add edge cases
3. **Test bugs** - Pick a bug and write a test that isolates the bug and shows it. (Likely you can easily fix the bug at this point.)
3. **Glossary expansion** - Add missing terms, improve definitions
4. **Example programs** - Create cool assembly examples that show off features
5. **Bug fixes** - Tackle issues from the KANBAN backlog

**Learning Path**

If you're new to programming:

1. Start with the documentation - you'll learn concepts while helping the project
2. Add tests - you'll understand how the parser works
3. Implement simple features - directives or pseudo-instructions
4. Work on complex features - symbol table management, optimization
5. Contribute to higher layers - VM or high-level language

If you're new to compiler stuff:

1. Start with documentation - you'll learn concepts while helping the project
2. Add tests - you'll understand how the parser works
3. Implement simple features - directives or pseudo-instructions
4. Work on complex features - symbol table management, optimization
5. Contribute to higher layers - VM or high-level language

If you're new to emulator development:

1. Start with the documentation - you'll learn concepts while helping the project
2. Add tests - you'll understand how the parser works
3. Implement simple features - directives or pseudo-instructions
4. Work on complex features - symbol table management, optimization
5. Contribute to higher layers - VM or high-level language

Starting to see a pattern here? ;D

**Community Standards**

We try to be welcoming and helpful:

- Ask questions! Seriously, we'd rather answer questions than fix bugs later
- Mistakes are how we learn - don't be afraid to try things
- Clear explanations beat clever code every time
- Teaching others is encouraged and appreciated
- Have fun - if you're not enjoying it, then consider if you are doing something wrong

Remember: KORE exists to make computer science accessible and fun. Every contribution should advance that mission. If you're not sure whether something fits, ask! We're happy to help you figure it out.