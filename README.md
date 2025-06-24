# Synchronization Primitives

- **Monitor**: Used with lock statements for mutual exclusion and signaling (Wait, Pulse, PulseAll).
- **SemaphoreSlim**: A lightweight, in-process version of Semaphore.
- **TaskCompletionSource**: Used for signaling between tasks, especially in async code.
- **ReaderWriterLockSlim**: Allows multiple threads to read or exclusive access for writing.
- **AutoResetEvent**: Signals a single waiting thread and then automatically resets.
- **ManualResetEvent**: Stays signaled until manually reset, releasing all waiting threads.


- **Mutex**: Provides exclusive access to a resource, can be used across processes.
- **Semaphore**: Limits the number of threads that can access a resource at once.
- **CountdownEvent**: Signals when a specified number of operations have completed.
- **Barrier**: Enables multiple threads to work in phases and wait for each other at a synchronization point.
 

- **SpinLock**: A lightweight lock for scenarios where threads wait for a very short time.
- **SpinWait**: Provides support for spin-based waiting.

Other topics
- **Lock (the lock keyword)** Syntactic sugar for Monitor.Enter / Monitor.Exit, making critical sections easier to write.
- **Concurrent Collections**
- **CancellationToken / CancellationTokenSource**

## Monitor
Monitor is a synchronization primitive that provides mutual exclusion and signaling between threads. It's the underlying mechanism used by the C# lock statement.
It ensures that only one thread at a time can enter a critical section, and supports:
- Enter / Exit for locking
- Wait, Pulse, and PulseAll for thread signaling (like condition variables)

Key Characteristics
Used for mutual exclusion (like lock).
- Supports waiting and signaling (like Wait, Pulse, PulseAll).
  -More fine-grained control than lock, but more verbose.

**Use Case: Printer Queue in an Office**
Scenario: Multiple employees send documents to a shared office printer. Only one document can be printed at a time.
- Business Logic: If the printer is busy, employees wait. When the printer finishes, the next in line is signaled.
- Monitor Role: Controls access to the printer and signals the next employee to proceed.
---
## SemaphoreSlim
SemaphoreSlim is a lightweight, faster version of Semaphore for use within a single process (i.e., within your application, not across multiple processes).
It's ideal when:
-You don’t need cross-process synchronization, and
-You want better performance and support for async/await.

Key Characteristics:
- Limits concurrent access to a resource within a process.
- Supports both synchronous (Wait) and asynchronous (WaitAsync) methods.
- Much faster and more efficient than Semaphore for in-process needs.

**Use Case: Limited License Software Access**
Scenario: A company has purchased 5 licenses for a data analysis tool. Up to 5 employees can use it simultaneously. Others must wait for a license to free up.
- Business Logic: Employees request access to the software. Only 5 can use it at once. The rest wait.
- SemaphoreSlim Role: Tracks and limits access. When one employee finishes, a spot opens up for someone else.
---
## TaskCompletionSource
TaskCompletionSource<TResult> is a way to create a manually controlled task — one that you can complete, fail, or cancel from the outside.
It’s commonly used to bridge non-async code with async workflows or to manually signal when some async result becomes available.

Key Characteristics
Used to create a task that’s completed manually.
- You can:
  - SetResult: mark as successful
  - SetException: mark as faulted
  - SetCanceled: mark as canceled
- Integrates perfectly with async/await.

**Use Case: Warehouse Awaiting Delivery Confirmation**

Scenario: A manager is waiting for a delivery confirmation from a courier. The manager’s task is on hold until the courier notifies them that the delivery is done.

- Business Logic: The manager doesn’t know when the delivery will happen, but wants to continue only after it's confirmed.
- TaskCompletionSource Role: The courier holds a special device that lets them trigger the confirmation whenever the delivery is done.
---
## ReaderWriterLockSlim
ReaderWriterLockSlim is a synchronization primitive that allows:
- Multiple threads to read from a resource at the same time, but
- Only one thread to write — and only when no one is reading or writing.
  It improves performance in scenarios with many more reads than writes.

Key Characteristics
- Multiple concurrent readers
- Single writer, and only when no readers or writers are active
- Ideal for read-heavy shared resources
- Supports:
  - EnterReadLock() / ExitReadLock()
  - EnterWriteLock() / ExitWriteLock()
  - EnterUpgradeableReadLock() (to switch to write if needed)

**Use Case: Office Document Archive**

Scenario: An archive room allows many employees to read files at once, but if someone wants to reorganize the files, they must lock the room exclusively.
- Business Logic: Reading is common and non-disruptive, so it's open to many. But writing (reorganizing) must be done carefully and without interference.
- ReaderWriterLockSlim Role: Lets many readers in, but blocks readers and writers when a writer is active.
---
## AutoResetEvent
Key Characteristics That Fit These Scenarios:
- Only one thread/action proceeds per signal.
- Automatic reset prevents duplicate execution.
- Ideal for sequenced processing or controlled access to resources.

**Use Case 1: Customer Support Ticket Escalation**

Scenario: A ticketing system assigns urgent issues to senior support agents.
- Business Logic: Only one urgent ticket should be assigned to a senior agent at a time, and once it's assigned, the system waits until the agent is ready for the next one.
- AutoResetEvent Role: Signals the dispatcher that the agent is available (sets the event). Once a ticket is handed off, the event auto-resets — no other ticket is assigned until the agent signals availability again.

**Use Case 2: Manufacturing – Assembly Line Step Trigger**

Scenario: In an automated assembly line, one machine must wait until the previous machine has finished a task.
- Business Logic: Machine B can only start once Machine A finishes its step — and only one part should move forward at a time.
- AutoResetEvent Role: Machine A signals (sets the event) when it finishes, allowing Machine B to proceed. Then the signal resets automatically, so the next part must wait for the next signal.
---
## ManualResetEvent ##
Key Characteristics That Fit These Scenarios:
- All waiting threads/actions are released when signaled.
- The event stays open (signaled) until manually reset.
- Ideal for broadcasting a go-ahead to many listeners.

**Use Case 1: Flight Boarding Gate Opens**

Scenario: An airport opens a gate for boarding.
- Business Logic: Passengers wait at the gate. Once the gate opens, all can start boarding.
- ManualResetEvent Role: Opening the gate is like setting the event — all waiting (and future) passengers can board until the gate is closed (reset manually).

**Use Case 2: System-Wide Configuration Update** 

Scenario: A software rollout that depends on a configuration file being updated.
- Business Logic: Several subsystems need to wait until the new configuration is available.
- ManualResetEvent Role: Once the configuration is updated, the event is set — all systems are free to continue. It stays "set" until another manual reset is needed for a future update.
---
## Mutex (Mutual Exclusion)
A Mutex (short for mutual exclusion) ensures that only one thread (or even process) at a time can access a shared resource.
Must be explicitly released using ReleaseMutex() — unlike lock, which auto-releases.
Unlike lock or Monitor, a Mutex can work across multiple processes, not just threads within a single application. 

Key Characteristics:
- Allows one thread or process at a time.
- Can be named, enabling cross-process coordination.

**Use Case: Company File Archive Access** 

Scenario: Imagine a secure filing cabinet that only one employee is allowed to open at a time — even if employees come from different offices (i.e., different processes).
- Business Logic: To avoid conflicts or data corruption, only one person should access the cabinet at a time. Others must wait until it's available again.
- Mutex Role: Acts like the key to the cabinet. Only the person holding the key can open it. Once they’re done, they release the key for someone else.
---
## Semaphore
A Semaphore controls access to a resource pool by limiting how many threads can enter at once. Unlike a Mutex (which allows only one), a Semaphore allows a set number of threads to access a section of code or resource concurrently.

Key Characteristics
-Allows a fixed number of concurrent accesses.
-Useful when multiple threads can safely access the resource (e.g., a limited pool).
-Must Release() after acquiring access with WaitOne().

**Use Case: Limited Checkout Counters in a Store**

Scenario: A store has 3 checkout counters, and customers wait in line. When a counter is free, the next customer steps up.
-Business Logic: At most 3 customers can check out at the same time. Others must wait.
-Semaphore Role: Acts like a counter controller. It tracks how many checkouts are in use and blocks others from entering until one becomes free.
---
## Countdown Event
A CountdownEvent is a thread synchronization primitive that allows one or more threads to wait until a certain number of operations are completed.
You initialize it with a count, and as different parts of your application complete their work, they signal the event by decrementing the count. When the count reaches zero, the waiting thread(s) are released.

Key Characteristics
- Initialized with a count.
- Each call to Signal() decreases the count by 1.
- Wait() blocks until the count reaches 0.
- Can be reset and reused.

**Use Case: Airport Boarding Final Call**

Scenario: A flight can’t take off until all passengers have boarded. The gate agent waits for check-ins from different entry gates.
- Business Logic: There are multiple boarding doors. Once all passengers are confirmed boarded from each door, the gate agent gives the final go-ahead.
- CountdownEvent Role: Keeps track of how many doors still need to report in. When the count reaches zero, the flight can take off.
---
## Barrier
Barrier is a synchronization primitive that enables multiple threads to work in parallel and then wait at a synchronization point (a barrier) until all threads reach that point — before any can proceed to the next phase.
It’s great for coordinating threads in phases — like multiple teams reaching milestones together.

Key Characteristics
- Coordinates multiple threads across phases.
- Each thread calls SignalAndWait() to indicate it finished the phase.
- All threads must arrive before any can continue.
- Can include a post-phase action (a method to run after each phase completes).

**Use Case: Construction Project Milestones**

Scenario: A construction company has three teams: Foundation, Framing, and Plumbing. They each work independently, but must all finish Phase 1 before any of them can begin Phase 2.
- Business Logic: No team can move to the next phase until everyone is done with the current one.
- Barrier Role: Coordinates teams so they all wait at the barrier until everyone finishes that phase — then all proceed together.
---
## SpinLock 
SpinLock is a lightweight mutual exclusion primitive where threads actively wait (spin) in a loop until the lock becomes available — instead of being put to sleep like with lock or Mutex.
It’s designed for very short critical sections where the overhead of putting a thread to sleep and waking it up would be more expensive than just spinning for a bit.

Key Characteristics
- Provides mutual exclusion, like lock, but doesn't block threads — they spin (busy wait).
- Best for very short, high-frequency critical sections.
- Manual care required to avoid performance issues (CPU burn) or deadlocks.
- Use with ref bool lockTaken for safe acquisition and release.

**Use Case: Reception Desk with Quick Service**

Scenario: There’s one receptionist. If a visitor arrives and sees someone at the desk, they just stand in line watching and waiting for their turn — rather than going away and coming back later.
- Business Logic: Everyone needs access, but each interaction is quick, so it's faster to just wait actively.
- SpinLock Role: Visitors keep checking if the desk is free — and grab it as soon as it is.
---
## SpinWait
SpinWait is a low-level, CPU-efficient primitive used for brief waiting loops where a thread does not immediately yield to the OS. It’s often used in custom spinlock implementations or performance-critical scenarios.
It balances performance and CPU usage by:
- Spinning (busy-waiting) at first,
- Then gradually yielding control (e.g., via Thread.Yield() or sleeping), if the wait continues too long.

Key Characteristics
- Good for very short waits (e.g., waiting for a flag).
- Avoids the overhead of Thread.Sleep() or kernel-based blocking.
- Adaptive: spins initially, yields if needed.

**Real-Life Analogy: Elevator Waiting Pattern**

Scenario: You're waiting for an elevator. At first, you look at the display every second (actively watching), but after a while, you get tired and just sit down to wait.
- Business Logic: When a wait is expected to be very short, it's more efficient to actively check.
- SpinWait Role: Starts with aggressive checking (spin), then backs off as time passes.
---

## Questions
- What are my alternatives to each primitive? What can I use to solve the same problem that AutoResetEvent does when I want to avoid Threads? If it does make sense.
- You have to download 100 songs. But you have to download only 3 at a time. How would you do it?