# AI-Assisted Development Prompts

## Original Project Prompt

Please help me to do this task: Requirements:

No frameworks, pure JS
CRUD operations for tasks (stored in browser storage - localStorage or IndexedDB)
Task properties: id, title, description, status, priority, dueDate, tags[]
Commands: add, list, update, delete, filter, search
Async operations with proper error handling
Input validation
Deliverables:

Git repository with commit history showing progression
README with usage instructions
Evidence of AI-assisted development (specs, prompts used). And save every prompt what I have used in files 'promts.md' (see below for file content)

## Session Prompts

### 1. Refactor to Vanilla JavaScript

Please refactor the JavaScript code from classes to be direct JavaScript without classes. Do not use async and try catches. Also add documentation or comments in code so it's easy to read.

### 2. Improve Code Structure

Can you give it structure now so it's easy to read and follow `script.js`? Make sure everything more important than other parts is at the start of the file. It should be easy to read and understand what's happening when you scroll down.

### 3. Simplify and Improve Readability

Is there any way we can make it more readable or easier to understand or simplify the code?

### 4. Improve Parameter Names

Can we give understandable parameter names?

Minu järgmine ül oleks: Full TypeScript conversion with strict mode
Custom type definitions for all entities
Generic utility functions (at least 3)
Add: recurring tasks, task dependencies, statistics
search, sorting
Category +-< Task >-+ Priority relationships
Deliverables:

TypeScript source with tsconfig.json  
 AI usage log: what worked, what didn't, kirjuta mida ma pean tegema ja kuidas see ära lahendada, nii, et kood oleks võimalikult lihtne ja loetav

Enne kui me alustamne võiks lisada siisa ka arhitektuuri ehk, DAL, BLL, UI folderid kuhu sisse me kirjutame vastavaad andmed ja, et filed ei lähe liiga
suureks siis võiks olla API-d ehk küsida infot
