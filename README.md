# CloudPosDemo README

## Introduction

The _CloudPosDemo_ package is C# software that implements the Afterpay Touch eServices 
CloudPOS javascript API.

eServices CloudPOS has been designed and built to be a single solution for eServices
such as gift cards, mobile phone recharge, lottery tickets, bill payments, transport
ticketing, and coupons for special offers and rewards.

The eServices CloudPOS client is an intelligent web page that is presented in a web
browser residing on your POS. eServices CloudPOS connects securely to popular
eservice suppliers such as Apple iTunes, Amazon, and Paysafecard over the internet to
supply activated products for your customers.

To use the CloudPOS javascript API you must have an environment that provides a browser
or a WebView and a means of injecting javascript API functions into the pages that are 
running in the browser, as well as capturing the event messages raised from with the 
javascript application.

## What's In CloudPosDemo

The _CloudPosDemo_ package consists of a number of components, which are intended to serve
as examples of how to access the CloudPOS javascript API from within .NET (C#) code.
It is intended to give a head-start to POS vendors whose own Point-of-Sale (Cash Register) 
software is written in C# or another .NET language.

More than being just an example, components within the _CloudPosDemo_ package encapsulate
the CloudPOS javascript API calls and wrap them in a .NET DLL.  These components may 
be incorporated straight into your code and used directly, easing the implementation even 
further.  

The _CloudPosDemo_ package, then, consists of the following components:
* **CloudPos** : This is the main Class Library (DLL) that encapsulates the javascript API calls
documented at [http://esp-api-docs.touchcorp.com/cloudpos/](http://esp-api-docs.touchcorp.com/cloudpos/).
The CloudPos Class Library exposes a main object, also called _CloudPos_, that offers methods
for all of the functions that a POS will need to perform, and raises .NET events to tell
the POS of the results.
* **CloudPosIE** : This Class Library provides a Windows Form containing a .NET WebBrowser
in which the javascript application runs, and which is used to present the user interface to
the POS operator.  This is distinct from the **CloudPos** DLL to allow for the possibility
of plugging in another user interface using a different browser widget.
* **CloudPosUI** : This Class Library contains a single Interface, _ICloudPosUI_, that the _UI_
class within **CloudPosIE** implements.  If **CloudPosIE** were to be replaced by another user
interface module, a class within that should implement _ICloudPosUI_.
* **Touch.Tools** : This Class Library contains a couple of static classes that provide generic utility
methods, including Extension methods for strings, and arrays.  There is also a class that
manages old-style Windows INI files.

The Class Libraries above, especially the **CloudPos** class library, are those that a POS
vendor may choose to incorporate directly into their code, with little or no changes.

In addition, the _CloudPosDemo_ package contains:
* **DummyPos** : This is a .NET executable which simulates the basic behaviour of a POS making 
calls upon the **CloudPos** DLL.  This executable is _NOT_ intended to be included in your own
project, but serves only as a working demonstration of how to interface to the supplied 
**CloudPos** DLL.
* **CloudSmartCards** and **Touch.SmartCards** : These two class libraries may be used if your POS
needs to be able to process smartcard products (such as GoCard transit cards in Queensland).
If SmartCard support *is* required, include the Reference to the **CloudSmartCards** library
in **CloudPosIE**, and un-comment the line "#define SMARTCARDS_SUPPORTED" in the *BrowserForm.cs*
file within the **CloudPosIE** project.

If running the entire package together, **DummyPos** should be the "Start Up" project.

## Use at Own Risk

The _CloudPosDemo_ package is intended primarily as example code, to guide your own 
POS integration with the CloudPOS javascript API.  As shown above, some of the components
you may find already fit for purpose, and you may choose them directly in your own application.

In doing so, you accept responsibility for maintaining the software, and use it at your own 
risk.  While Afterpay Touch will willingly assist you in your POS integration, they do 
not guarantee the performance of any software within the _CloudPosDemo_ package, nor warrant
it will run well in your environment.  Afterpay Touch may provide upgrades and maintenance fixes 
to this software, but do not promise to do so.

The supplied code has been written and tested against the .NET 4.5 Framework.  You are free
to try to make it work with other versions of the .NET Framework, if that suits you better.
Again, do so at your own risk.

