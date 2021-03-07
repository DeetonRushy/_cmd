# :smiley: How to use cmd

This project isn't all that confusing but there are certain things that may be hard to workout.

## Commands

Commands are registered as either a Command object or CommandWithArguments object. If a command that doesn't take arguments in ran with arguments, we automatically assume you're trying to run a command that takes arguments, so you'll receive an error. For example, 'time' is a command that takes no arguments. I'll run it now and see what we get.

![](https://cdn.discordapp.com/attachments/715986045279141949/818139628774096966/unknown.png)
It outputs the time, as we expect. Lets try the same thing but pass arguments to it.

![](https://cdn.discordapp.com/attachments/715986045279141949/818140284398993418/unknown.png)

This is the same with commands with arguments, if you try to run one without arguments you'll receive an error.

## Cvars

Cmd's Cvar system is basically a way for the end user to edit variables. You can create a cvar and then reference it in your code instead of referencing a variable. This should only be used for non-critical variables, such is the console title, width etc. When creates a cvar, you create a lambda expression that is ran every-time something attempts to modify that variable. If you return RetType._C_SUCCESS from the lambda, it is changed. Otherwise, the value is left how it was.

### How to use them

To view the value of a cvar, simply type the name of it.

![](https://cdn.discordapp.com/attachments/715986045279141949/818141708097028116/unknown.png)

To attempt to modify the value of a cvar, type the name of it and then the value.

![](https://cdn.discordapp.com/attachments/715986045279141949/818142050662481980/unknown.png)

### For developers

![Create Cvar](https://cdn.discordapp.com/attachments/715986045279141949/818142391374577694/unknown.png)

```
```
| Argument    | Description |
| ---         | ----------- |
| 1        | The name of the con-var. |
| 2  | The lambda to execute whenever something attempts to change the value. |
| 3 | The initial value for the con-var to hold. |
| 4 | Brief description of the value and what power it holds. |

This is quite powerful because it lets the user decide configuration for certain aspects of the program.
