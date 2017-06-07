Datepicker Tag Helper

ASP .NET Core Tag Helper for datepicker. Uses [Bootstrap 3 Datepicker v4](https://eonasdan.github.io/bootstrap-datetimepicker).

## Getting Started

### Installing
1. install nuget package

```
Install-Package JezekT.AspNetCore.Bootstrap.Datepicker -Pre
```

2. make Tag Helper available to the view by adding following line to Views/_ViewImports.cshtml

```
@addTagHelper *, JezekT.AspNetCore.Bootstrap.Datepicker
```

3. add Tag Helper to the view

```html
<datepicker datetime-property="Expiration" format="L" class="form-control"></datepicker>
```

>*datetime-property* - use any DateTime property from your model;

>*format* - use [format](https://eonasdan.github.io/bootstrap-datetimepicker/#custom-formats) string;

4. add CDN Tag Helper to the view

```html
<datepicker-jscss></datepicker-jscss>
```

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

