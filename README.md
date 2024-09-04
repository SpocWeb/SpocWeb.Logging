# SpocWeb.Interfaces
This Project provides the most fundamental Interfaces, Delegates, Attributes, Exceptions, Enums and Classes
that are useful in most other Projects.

This is similar to the role that mscorlib.dll or System.dll play for any .NET Application,
but adds many more Definitions and Utilities that are missing in these and other Assemblies.

By declaring them in a single, relatively small Library,
other Projects can re-use these Definitions
without having to define their own, duplicate or even conflicting Classes.

Used consistently, these Definitions allow for very declarative Code
and save unnecessary (and risky) Data transforms. 

## Attributes
to declaratively link semantically connected Classes e.g.
	* [DisplayStringAttribute] to localize Enum Values and add Descriptions/Tool-Tips.
	* [DomainAttribute] to describe the Domain of Function Arguments, and its Inverse, the
	* [ImageAttribute] to describe the Range of a Function.
	* [InverseOfAttribute] to indicate the inverse Function.
	* [DerivativeOfAttribute] to indicate the derivative Function and its Inverse, the
	* [IntegralOfAttribute] to indicate the Integral Function.

## Interfaces
Interfaces and Class Hierarchies enable Polymorphism and generic Algorithms.
Defining them in a shared library allows for broad reuse and finding new applications for them:
* IRange, IInterval and ISegment enable Interval Arithmetic
* IKeyed allows to uniquely identify an Entity whereas
* IKeyedGetIf describes a Function that 
* IReader is a high-performance Stream Interface for individual Values

## Delegates
When a single Method is sufficient as an Interface,
you can also define the equivalent Delegate and implement generic Algorithms on it.
Delegates have advantages compared to Interfaces:
* the same Class can provide multiple alternative Delegates of the same Type
* Delegates are more flexible to cast, because early on .Net allowed for CoVariance on the Result
  and ContraVariance in the Parameters .
Examples are:
* DReader, the equivalent Value Stream to IReader
* DKeyedGetIf, the equivalent Value Stream to IKeyedGetIf 

## Extension Classes
Extension Methods allow for IntelliSense and intuitive Object-based Syntax,
but are more modular than OO Classes, reducing Coupling.
.Net requires static Classes to define Extension Methods,
but beware of unnecessarily coupling independent Methods
by placing them into the same Class or Assembly.
Examples are:
* Should which defines readable Assertions for Tests but also Pre- and Post-Conditions.
* XString which is useful almost everywhere
* XDouble and XFloat which define Assertions and allow for fluent Arithmetic

## Enums
Several widely used Enums are defined, like:
* Css defines a lot of Formatting Enums like Font, Color, Style etc.
* Logg.Level for granular Log Control and hook for Log-Extensions.

# License
[![Hippocratic License HL3-BDS-BOD-ECO-MEDIA-MIL-MY-SOC-SUP-SV-TAL-XUAR](https://img.shields.io/static/v1?label=Hippocratic%20License&message=HL3-BDS-BOD-ECO-MEDIA-MIL-MY-SOC-SUP-SV-TAL-XUAR&labelColor=5e2751&color=bc8c3d)](https://firstdonoharm.dev/version/3/0/bds-bod-eco-media-mil-my-soc-sup-sv-tal-xuar.html)
This Software is licensed by the [Hippocratic License](https://firstdonoharm.dev),
because we feel that technology is not neutral, but can be abused.

Although we apply a permissive License for derivative Work,
we hope that other developers follow our example
and choose [similar ethical licenses](https://ethicalsource.dev/licenses/) for derivative works.

# Peculiarities

## Unit Tests in Libraries
Unit Tests often provide excellent examples for the usage of Functions.
Therefore we often add Tests to the libraries,
which has the additional benefit that they are self-validating.

In the case of [Pure] Functions only the Parameters determine the Result,
which can be tested using [TestCaseAttribute] or [TestCaseSourceAttribute].

Such Functions can be tested without further Code,
which maximizes the ratio of benefit by effort.

Such [TestCaseAttribute]s give a good impression of the Function
and save redundant Descriptions.
For examples, see
* XDouble.Bell
* XDouble.Partition
* XDouble.Hypot
* XDouble.SinPi
* XDouble.Logistic
* XDouble.SoftPlus
etc.

## Documentation
The Code is heavily documented, because even with fluent and well-named Code,
essential Information and Context is often missing.

Unit-Tests can provide Examples for usage,
but Comments are needed to document the reasons for Design Decisions made during development.

Without these Comments much of the effort that went into designing is lost.
We have seen new developers introduce inferior changes that had already been ruled out.
As [George Santayana](https://en.wikiquote.org/wiki/George_Santayana) wrote:
**Those who cannot remember the past are condemned to repeat it.**

NDoc Comments provide the means for that and can be generated into separate Documentation.

Comments **within** Code are less useful and can be distracting.
Try to split up the Code up into smaller Methods.
That allows to use NDoc and at the same time it usually leads to CLEANer Code.

It is important to keep the Documentation out of the way for knowledgable Developers, therefore:
* NDoc is typically collapsed when opening a file
(according to the Visual Studio Setting in Tools\Options\Text Editor\C#\Advanced: "Enter outlining mode")
* Restrict the Summary to a meaningful and comprehensive single Line, like this. 
<code lang='cs'>
	/// &lt;summary> Restrict the Summary to a single Line &lt;/summary>
</code>
Place any additional Information into the Remarks or Returns Sections.
If you have a hard time, restricting yourself,
this may indicate that the Method or Class has more than one responsibility. 
* keep Comments brief, so they can be placed at the End of a Code Line, to reduce the cognitive Impact.
* Don't enforce to comment every Member, but for Interfaces and Classes at least a Summary should be mandatory.
* Use &lt;inheritdoc /> to avoid redundant Documentation. Visual Studio resolves this for ToolTips.

This is not literate Programming as propagated by Donald Knuth,
where the Code is subordinated to the Structure of Exposition.

### Documentation Priority
There is a clear Priority when improving the Documentation in Code:

#### 1. Naming:
Good Naming of Classes, Members and Methods provide the best Documentation possible,
because it also enforces documentation on any Client-Code.

Fluent APIs drive this even further: they enable a human-readable DSL (Domain-specific Language)

#### 2. MetaData: Attributes and Interfaces
Non-obvious Semantics should be expressed using well-known Interfaces or add Attributes.
Prominent Examples are:
* IEnumerable{T} for sequential Access to value Streams
* IReadOnlyList{T} for random access Collections
* [StringLengthAttribute] and other Attributes in the System.ComponentModel.DataAnnotations? namespace.
* [DisplayStringAttribute] to localize Enum Values and add Descriptions/Tool-Tips.
* [DomainAttribute] and [ImageAttribute] to indicate Parameter and Result Ranges.
* [InverseAttribute] to link Functions and their Inverses

These Attributes can be inspected via Reflection and allow for subsequent Automation or Code Generation.

Feel free to invent your own Attributes to mark up and add MetaData to your Artifacts.

#### 3. NDoc Comments:
NDoc provides for structured Documentation that can be linked to and from Source-Code
and Documentation in other Formats (usually MarkDown).

The Proximity to the annotated Code helps both Developers and Users in maintaining and navigating the Code.
Especially Links to dynamic Instances only available at runtime can are helpful to understand the Code
without having to debug it.

A *.xml File can be generated from this Documentation and analyzed, similar to Attributes,
but the Text-Nature of NDoc Comments and loose Validation makes it unreliable for such purposes.

#### 4. Unit-Tests and Sample Projects:
As [described above](#Unit Tests in Libraries), Unit Tests provide for some of the best Documentation.
It can also be linked into the NDoc, where Processors like DocFX can weave it into the Documentation.

Especially [Pure] Functions can be documented extensively using [TestCaseAttribute]s. 

#### 5. Inline-Comments:
Comments **in** Code are less useful,
but they can often be converted into NDoc by splitting Code up into smaller Methods,
which also supports CLEANer Code.

When adding/leaving Comments in Code it is important to move them out of the way of regular Code.

Brief Comments are usually //placed at the End of a Code Line, to reduce the cognitive Impact.


## Static Extension Methods
Extension Methods are Feature of functional Programming that was introduced with C# 3.
It allows to call and find Methods (via IntelliSense) using OO Dot-Notation,
which greatly improves Developer Experience.
In these Libraries Extension Methods for the same interface are usually collected
in a static Class named after the interface replacing the 'I' Prefix by an 'X' like this:
* XKeyed for IKeyed
* XInterval for IInterval
* XChar for Char

This keeps the Name brief and recognizable,
unlike the common ...Extension Naming, which places undue emphasis on the Suffix.

### As...() Extension Methods instead of Conversion Operators
C# allows to declare implicit and explicit Conversion of Values between Types,
but these are not directly visible to the Developer.
Additionally these couple the Types, so it is not possible to use one without the other.
Therefore it is better to define these Conversions as Extension Methods
that can be left away when only one Class is needed.

These Extension-Conversion-Methods start with As... and end with the Target Type Name:
<code lang='cs'>public static Target AsTarget(this Source value) { ... }</code>

This eliminates Coupling and allows Developers to find it with IntelliSense,
by typing '.As' and choose the proper Conversion from the Dropdown List.
(Or by pressing Alt+'.' (Visual Studio) or Ctrl+Space (Resharper))

## Standalone partial Class Files
To further decrease coupling and increase reusability,
many core Classes are made partial and split into 2 or more Files:
* a reusable *.cs Implementation and
* a *.public.cs File making the class ``public partial class ...``

Examples:
* XString.cs
* Arrays.cs
* Css.cs
* XDouble.cs
* Should.cs

This allows to link/include the *.cs File in other Projects
without the need to reference the full Assembly/NuGet Package.
Since these Classes are internal by Default, they are not visible in the public API
which prevents Name Clashes between different Assemblies using the same partial Classes. 

Since Enums cannot be declared as partial,
all Enums are declared as inner Enums of partial Classes.
This allows to use the same Enum Values internally and ensure Consistency
among Libraries and with the Definition here.

In public Interfaces, these internal Enums can be exposed either as their associated Value Type
(int by default) or a Buddy-Enum with the same integer Values and preferably the same Names.
Consistency can be ensured by a Pair of Conversion (Extension-)Functions between both. 

